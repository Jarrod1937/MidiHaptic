Imports System.IO
Imports System.IO.Ports
Imports Microsoft.DirectX
Imports Microsoft.DirectX.AudioVideoPlayback
Imports System.Threading
Imports System.Text

' Class reads and parses loaded midi file. Thankfully midi files are big-endian
' which makes parsing them much easier than small-endian.
Public Class midihaptic

    Private totalkeys As Int16 = 108
    Private blackkeypos As New List(Of Int16)(New Int16() {22, 25, 27, 30, 32, 34, 37, 39, 42, 44, 46, 49, 51, 54, 56, 58, 61, 63, 66, 68, 70, 73, 75, 78, 80, 82, 85, 87, 90, 92, 94, 97, 99, 102, 104, 106})

    Private maxtickval As Int32 = 0
    Private mintickval As Int32 = 0

    ' Filename and location of file currently loaded
    Private loadedfilename As String = ""

    ' When running through header, store timediv header HEX for later processing
    Private timeheader As String = ""

    ' Set tickpos to default 0
    ' Tick position is a delta of time multiplied by the
    ' tick time to arrive at the actual time
    Private tickpos As Int32 = 0
    ' Track format
    ' 0 is single, 1 is multi, 2 is multi-single
    ' If 0, ftrackc MUST be 1
    Private fformat As Int16 = -1
    ' Track count
    Private ftrackc As Int16 = 1

    ' Time delta div value
    Private ftdiv As Int16 = 0
    ' 0 is ticks per beat, 1 is frames per second
    ' default is 0, val more than likely will be set in runtime per file
    Private ftdivtype As Int16 = 0
    ' beats per minute, default to 120, if meta event is available, will override value
    Private bpm As Int16 = 120

    ' ticks per quarter note
    Private tpq As Int16 = 24

    ' Milliseconds per beat
    Private mpb As Int32 = 500000
    Private ticktime As Double = 8333.33

    ' Default to 24 fps if fps is applicable
    ' Will override with any specified value in header
    Private fpsval As Decimal = 24

    ' Is the file a badly formatted one?
    ' Set during header reading if something is awry
    Private badfile As Boolean = False

    ' track string array, full track hex string for each track is stored here
    Private tracka As New List(Of String)

    ' All track data is read into the array below
    ' which is then used for playing the music
    Private compressedtrack As New List(Of String)
    Private compressedtracktimes As New List(Of Int32)
    Private compressedtrackvel As New List(Of Int32)
    Private activekeys As New List(Of Int16)
    Private activekeysvel As New List(Of Int32)
    Private activekeyswhand As New List(Of Int16)
    Private activekeyswhandf As New List(Of Int16)
    Private activekeyswhandh As New List(Of String)
    Private activekeyswhandfc As New List(Of Int16)
    Private activekeyswhandvel As New List(Of Int32)

    ' Running status tracking, skips the status messages after it's been recorded once
    Private runningstatus As String = 0
    Private runningstatusactive As Boolean = False

    ' New tracks are scaled to the configured ticktime
    Private newtrack As New List(Of String)
    Private newtracktimes As New List(Of Int32)
    Private newtrackvel As New List(Of Int32)
    Private newtrackscalevel As Int16 = 0

    ' How many active keys are there at each tick pos
    Private keycount As New List(Of Int32)

    ' Time in milliseconds. Compressing the time also has the benefit of quantizing the midi file
    Private compressedtime As Int32 = 125

    ' Max key span of person, user configurable
    Private maxspan As Int16 = 9

    ' Standard width of white key on most pianos, in inches
    Private widthwhitekey As Double = 0.9252
    Private widthblackkey As Double = 0.5394

    ' Max distance based on userentered finger span
    Private maxdistance As Double = Math.Floor((maxspan * widthwhitekey) + 0.1)

    ' After research and measurement, the general ratio between max key span
    ' and individual finger span is... 0.35275 
    ' (could be entirely wrong, ratio from a limited sample of people)
    Private individualratio As Double = 0.35275
    Private maxfingerdistance As Double = maxdistance * individualratio

    ' How many keys to the left of the righthandthresh should we consider the
    ' right hand range
    Private righthandrange As Int16 = 4


    Private Sub initsub(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        updatestatus(0)
        calcfingerspan()
        tbplayspeed.Value = 1

        Dim ports As String() = SerialPort.GetPortNames()
        For Each port As String In ports
            serialporttxt.Items.Add(port)
        Next port

        If serialporttxt.Items.Count() > 0 Then
            serialporttxt.SelectedIndex = serialporttxt.Items.Count() - 1
        End If

        serialopen()
    End Sub

    Private Sub closingsub(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
        serialclose()
    End Sub


    ' Based on the midi standards, identify which tick div type is set for file
    ' and calculate the ticktime per tick position
    Private Sub setticktime()
        If ftdivtype = 0 Then
            ticktime = mpb / ftdiv
        Else
            Dim lowerbits As String = converttobinary(timeheader.Substring(0, 2))
            lowerbits = lowerbits.PadLeft(8, "0")
            Dim lowerbitsval As Int16 = Convert.ToInt16(lowerbits.Substring(1, 7), 2)

            If lowerbitsval > 0 Then

                If lowerbitsval = 29 Then
                    fpsval = 29.97
                Else
                    fpsval = lowerbitsval
                End If
            End If

            Dim ticksperframe As Int16 = Convert.ToInt16(timeheader.Substring(2, 2), 16)
            ticktime = (1000000 / (fpsval * ticksperframe))
        End If

        If compressedtime < (ticktime / 1000) Then
            compressedtime = ticktime / 1000
        End If

        setfinaltime()
        updatespeedtime()
    End Sub

    ' Final time is the most compressed form of the track data that is based on
    ' a seek window of 'compressedtime'. This is not a proper sampling frequency.
    ' however, this extends the midi events to a time scale that can be played back
    ' at reasonable reproduction with a lower resolution timer
    Private Sub setfinaltime()

        Dim offsetc As Int32 = 0
        Dim offset_resetc As Int32 = Math.Floor(compressedtime / (ticktime / 1000))
        Dim totc As Int32 = 0

        Dim maxval As Int32 = compressedtracktimes.Max

        While totc <= maxval

            Dim maxvalue As Int32 = (totc + offset_resetc)
            Dim minvalue As Int32 = totc
            Dim keyc As Int32 = 0

            Dim tempkeyc As New List(Of String)

            For timeindex As Integer = 0 To compressedtracktimes.Count - 1

                If compressedtracktimes(timeindex) >= minvalue And compressedtracktimes(timeindex) < maxvalue Then
                    newtrack.Add(compressedtrack(timeindex))
                    tempkeyc.Add(compressedtrack(timeindex))
                    newtracktimes.Add(offsetc)
                    newtrackvel.Add(compressedtrackvel(timeindex))
                    keyc += 1
                End If

            Next
            keycount.Add(keyc)



            offsetc += 1
            totc += offset_resetc

        End While

        newtrackscalevel = 127 - newtrackvel.Average()
        maxtickval = newtracktimes.Max()
    End Sub

    ' delta time and a few other values are variable length
    ' up to 4 bytes, using the most significant bit as a flag to indicate more bytes
    ' are to follow. If 0, no more, if 1, at least one more byte.
    ' Actual value is the concatination of the remaining 7 bits for all bytes in the field.
    ' Function returns an array, the first element is the byte count, the second is the variable value itself
    Private Function getvarlen(ByVal bytestr As String)
        Dim returnval() As String = {"", ""}

        If bytestr.Length > 8 Then
            bytestr = bytestr.Substring(0, 8)
        End If

        Dim bytearr() As String = slice(bytestr, 2)
        Dim binstr As String = ""
        Dim bytec As Int16 = 0

        For Each bytevalue In bytearr

            Dim binbuff As String = converttobinary(bytevalue)

            binbuff = binbuff.PadLeft(8, "0")

            binstr += binbuff.Substring(1, binbuff.Length - 1)
            bytec += 1

            If msbval(binbuff) = 0 Or bytec >= 4 Then
                Exit For
            End If
        Next

        returnval(0) = bytec
        Dim temphex As String = Convert.ToString(Convert.ToInt32(binstr, 2), 16).ToUpper

        If temphex.Length = 1 Then
            temphex = "0" & temphex
        End If

        returnval(1) = temphex

        Return returnval

    End Function

    ' For each track, read all events
    Private Sub readevents()

        For index As Integer = 0 To tracka.Count - 1
            trackeventread(tracka(index), 0)
        Next

    End Sub

    ' Walk through track and record track data in 
    ' compressedtrack var.
    Private Sub trackeventread(ByVal trackstr As String, ByVal ctracktime As Int32)


        While trackstr.Length > 0
            Dim byteoffset As Int16 = 0

            Dim eventlen() As String = getvarlen(trackstr)

            ' Set current byteoffset and increase ctracktime by event delta t
            byteoffset += Convert.ToInt16(eventlen(0))
            ctracktime += Convert.ToInt32(eventlen(1), 16)

            trackstr = trackstr.Substring(byteoffset * 2, trackstr.Length - (byteoffset * 2))

            If trackstr.Length > 2 Then

                If trackstr.Substring(0, 2) = "FF" Then
                    ' Is a meta event
                    Dim metatype As String = trackstr.Substring(2, 2)
                    trackstr = trackstr.Substring(4, trackstr.Length - 4)

                    Dim metaeventlen() As String = getvarlen(trackstr)

                    Dim metalenbytec As Int16 = Convert.ToInt16(metaeventlen(0))
                    Dim metalen As Int32 = Convert.ToInt32(metaeventlen(1), 16)

                    Dim clen As Int32 = (trackstr.Length / 2)
                    Dim elen As Int32 = ((metalen))
                    If clen >= elen Then
                        Dim metacontent As String = trackstr.Substring(metalenbytec * 2, metalen * 2)

                        If metatype = "51" Then
                            bpm = 60000000 / Convert.ToInt32(metacontent, 16)
                            mpb = Convert.ToInt32(metacontent, 16)
                        End If

                        Dim metastroffset As Int32 = ((metalenbytec * 2) + (metalen * 2))

                        trackstr = trackstr.Substring(metastroffset, trackstr.Length - metastroffset)
                    Else
                        MsgBox("Track reading encountered an error, track may be truncated")
                        trackstr = ""
                    End If
                Else
                    'If trackstr.Length > 2 Then
                    ' is an actual music event
                    Dim eventtype As String = trackstr.Substring(0, 1)

                    If eventtype = "8" Or eventtype = "9" Or eventtype = "C" Or eventtype = "D" Or eventtype = "A" Or eventtype = "B" Or eventtype = "E" Then
                        runningstatus = eventtype
                        Dim eventchannel As String = trackstr.Substring(1, 1)

                        Dim eventbc As Int16 = 1

                        Dim eventparam1 As String = trackstr.Substring(2, 2)

                        eventbc += 1
                        Dim eventparam2 As String = "7F"
                        ' Param2 is not used by event type C or D
                        If eventtype <> "C" And eventtype <> "D" Then
                            eventparam2 = trackstr.Substring(4, 2)
                            eventbc += 1
                        End If

                        trackstr = trackstr.Substring((eventbc * 2), trackstr.Length - (eventbc * 2))

                        If eventtype = "9" Then
                            Dim velvalue As Int32 = Convert.ToInt32(eventparam2, 16)

                            If velvalue <> 0 Then
                                compressedtrack.Add(eventtype & "_" & eventparam1)
                                compressedtracktimes.Add(ctracktime)
                                compressedtrackvel.Add(Convert.ToInt32(eventparam2, 16))
                            End If
                        End If

                    Else
                        ' No valid event type is given, thus must be a running status
                        runningstatusactive = True

                        ' This time there is no event type and controller byte, so starts out as 0
                        Dim eventbc As Int16 = 0

                        ' Grab param1
                        Dim eventparam1 As String = trackstr.Substring(0, 2)

                        eventbc += 1
                        Dim eventparam2 As String = "7F"
                        ' Param2 is not used by event type C or D
                        If runningstatus <> "C" And runningstatus <> "D" Then
                            eventparam2 = trackstr.Substring(2, 2)
                            eventbc += 1
                        End If

                        trackstr = trackstr.Substring((eventbc * 2), trackstr.Length - (eventbc * 2))

                        If runningstatus = "9" Then
                            Dim velvalue As Int32 = Convert.ToInt32(eventparam2, 16)

                            If velvalue <> 0 Then
                                compressedtrack.Add(runningstatus & "_" & eventparam1)
                                compressedtracktimes.Add(ctracktime)
                                compressedtrackvel.Add(velvalue)
                            End If
                        End If

                    End If

                End If

            End If

        End While

    End Sub

    ' Slice a string into a string array by a fixed width of characters
    Function slice(ByVal text As String, ByVal length As Integer) As String()

        If text.Length Mod 2 <> 0 Then
            text += "0"
        End If

        Dim strArray As String() = New String((text.Length \ length) - 1) {}

        For i As Integer = 0 To Convert.ToInt32(text.Length / length) - 1
            strArray(i) = text.Substring(i * length, length)
        Next

        Return strArray
    End Function


    ' Read midi track length, read track to specified length and strip out length bytes
    Private Sub striptracklen()
        Try
            For index As Integer = 0 To tracka.Count - 1

                If tracka(index).Length > 8 Then
                    Dim tracklenval As Int32 = Convert.ToInt32(tracka(index).Substring(0, 8), 16)
                    tracka(index) = tracka(index).Substring(8, tracklenval * 2)
                Else
                    ' If not > 8, then bad track
                    tracka(index) = ""
                End If
            Next
        Catch e As Exception
            MsgBox("Error reading the file has occured, usually caused by a badly formated file (or coding error on my part!). Error is: " & e.ToString())
        End Try

    End Sub

    ' Read track data per track from file
    ' Total track count pulled from header
    ' Originally read all track data into large string and used a split function
    ' However, that resulted in truncating of data (probably internal split function limitations).
    Private Sub readtracks(ByVal initialoffset As Int32, ByVal trackpos As Int16)
        Using reader As New BinaryReader(File.Open(loadedfilename, FileMode.Open))
            Dim byoffset As Int32 = initialoffset
            reader.BaseStream.Seek(byoffset, SeekOrigin.Begin)
            ' Loop through length of file.
            Dim pos As Integer = byoffset
            Dim length As Integer = reader.BaseStream.Length

            Dim fullbytestr As String = ""

            While pos < length
                Dim value As String = Conversion.Hex(reader.ReadByte())

                ' Pad single 0 bytes as 00
                If value.Length = 1 Then
                    value = "0" & value
                End If
                fullbytestr += value
                pos += 1

                If fullbytestr.Contains("4D54726B") Then
                    fullbytestr = fullbytestr.Substring(0, fullbytestr.Length - 8)
                    Exit While
                End If
            End While

            tracka.Add(fullbytestr)

            initialoffset += (fullbytestr.Length / 2) + 4
        End Using

        trackpos += 1

        If trackpos <= ftrackc Then
            readtracks(initialoffset, trackpos)
        End If
    End Sub

    ' Read midi header
    Private Sub readheader()
        Using reader As New BinaryReader(File.Open(loadedfilename, FileMode.Open))
            Dim recordwidth As Int32 = 18
            reader.BaseStream.Seek(0, SeekOrigin.Begin)
            Dim pos As Integer = 0
            Dim headerstr As String = ""

            Dim length As Integer = recordwidth

            While pos < length
                ' Read the integer.
                Dim value As String = Conversion.Hex(reader.ReadByte())

                ' Pad single 0 bytes as 00
                If value.Length = 1 Then
                    value = "0" & value
                End If
                headerstr += value

                pos += 1
            End While

            Dim reportedhl As Int16 = Convert.ToInt16(headerstr.Substring(8, 8), 16)

            ' If header does not contain MThd in hex, or reported header length
            ' is not 6, then probably a bad file
            If headerstr.Substring(0, 8) <> "4D546864" Or reportedhl <> 6 Then
                badfile = True
            End If


            ' Extract and store important parameters from header
            fformat = Convert.ToInt16(headerstr.Substring(16, 4), 16)
            ftrackc = Convert.ToInt16(headerstr.Substring(20, 4), 16)
            timeheader = headerstr.Substring(24, 4)
            ftdiv = Convert.ToInt16(timeheader, 16)
            Dim binstrtemp As String = converttobinary(timeheader.Substring(0, 2))
            binstrtemp = binstrtemp.PadLeft(8, "0")
            ftdivtype = msbval(binstrtemp)


        End Using

    End Sub

    ' Converts HEX to binary
    Private Function converttobinary(ByVal hextext As String)
        Dim returnval As String = ""

        If hextext.Length <> 0 Then
            returnval = Convert.ToString(Convert.ToInt32(hextext, 16), 2)
        End If

        Return returnval
    End Function

    ' Return leading binary value, used as a flag in a few events
    ' and used as a flag for variable length values
    Private Function msbval(ByVal binstr As String)

        Dim returnint As Int16 = -1

        If binstr.Length <> 0 Then
            returnint = Convert.ToInt16(binstr.Substring(0, 1))
        End If

        Return returnint
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim test As String = ""
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnplay.Click
        updatespeedtime()
        ticktimer.Enabled = True
        drawsub()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnstop.Click
        tickpos = mintickval
        ticklabel.Text = tickpos
        ticktimer.Enabled = False
    End Sub

    Private Sub ticktimer_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles ticktimer.Elapsed

        ' Turn off tick timer if there are no more events
        If tickpos > maxtickval Then
            If cbrepeat.Checked = False Then
                ticktimer.Enabled = False
            End If

            tickpos = mintickval
            ticklabel.Text = mintickval
        Else
            timeraction()
        End If

        ' update time
        ltime.Text = timefromseconds(tickpos * (ticktimer.Interval / 1000))
    End Sub


    Private minkeys() As Int32 = {0, 0}
    Private maxkeys() As Int32 = {0, 0}
    Private minkeysavg() As Int32 = {0, 0}

    Private Sub clearfingers()
        ' Clear vars
        activekeyswhand.Clear()
        activekeyswhandf.Clear()
        activekeyswhandfc.Clear()
        activekeyswhandh.Clear()
        activekeyswhandvel.Clear()
    End Sub

    Private Sub clearrightfingers()

        Dim removestart As Int32 = activekeyswhandh.IndexOf("R")
        Dim removeend As Int32 = (activekeyswhandh.Count) - (removestart)

        If removestart >= 0 Then
            activekeyswhand.RemoveRange(removestart, removeend)
            activekeyswhandf.RemoveRange(removestart, removeend)
            activekeyswhandfc.RemoveRange(removestart, removeend)
            activekeyswhandh.RemoveRange(removestart, removeend)
            activekeyswhandvel.RemoveRange(removestart, removeend)
        End If
    End Sub



    Private Sub handprocess(ByVal handlist As List(Of Int16), ByRef keyact As List(Of Int16), ByRef keyvel As List(Of Int32), ByVal isleft As Boolean)

        Dim offsetnum As Int16 = 0
        Dim assignfc As Int16 = 6

        While assignfc > 5 And offsetnum <= 10

            If isleft = True Then
                clearfingers()
            Else
                clearrightfingers()
            End If

            assignfc = assignfingers(handlist, keyact, keyvel, isleft, offsetnum, True)
            offsetnum += 1
        End While

        If offsetnum > 10 Then

            If isleft = True Then
                clearfingers()
            Else
                clearrightfingers()
            End If

            assignfc = assignfingers(handlist, keyact, keyvel, isleft, 0, False)
        End If

    End Sub



    Private Function assignfingers(ByVal handlist As List(Of Int16), ByRef keyact As List(Of Int16), ByRef keyvel As List(Of Int32), ByVal isleft As Boolean, ByVal minoffset As Int16, ByVal usemin As Boolean)

        Dim fingersassigned = 0

        Dim handval As String = "L"
        Dim cfinger As Int16 = 0
        Dim keybuff As Int16 = 0

        If isleft = False Then
            handval = "R"
            cfinger = 5
        End If

        If isleft = False And handlist(0) = minkeys(1) Then
            usemin = False
        ElseIf isleft = True And handlist(0) = minkeys(0) Then
            usemin = False
        End If

        If usemin = True Then

            If isleft = False Then
                cfinger = findfingerpos(cfinger, minkeysavg(1) + minoffset, handlist(0))
            Else
                cfinger = findfingerpos(cfinger, minkeysavg(0) + minoffset, handlist(0))
            End If

        End If

        For i As Integer = 0 To handlist.Count - 1
            cfinger = findfingerpos(cfinger, keybuff, handlist(i))

            If isleft = True And cfinger = 5 And blackkeypos.Contains(handlist(i)) Then
                Exit For
            ElseIf isleft = False And cfinger = 6 And blackkeypos.Contains(handlist(i)) Then
                cfinger += 1
            End If

            activekeyswhand.Add(handlist(i))
            activekeyswhandf.Add(cfinger)
            activekeyswhandfc.Add(1)
            activekeyswhandh.Add(handval)

            Dim velindex As Int32 = keyact.IndexOf(handlist(i))
            activekeyswhandvel.Add(keyvel(velindex))

            keybuff = handlist(i)
        Next

        fingersassigned = cfinger

        If isleft = False Then
            fingersassigned -= 4
        End If

        Return fingersassigned
    End Function


    Private nodraw As Boolean = False

    ' Primary sub that executes at each compressed time tick
    Private Sub timeraction()
        Dim actionstr As String = ""
        ticklabel.Text = tickpos

        ' Find all events where tick position equals tickpos
        ' Needed in this fashion as there more than likely are multiple
        ' simultaneous tracks
        Dim result = Enumerable.Range(0, newtracktimes.Count).Where(Function(i) newtracktimes(i) = tickpos).ToList()

        ' Loop through results on tickpos search to extract each event
        For Each timeindex In result
            ' Activekeys is a list that keeps track of all existing ON events
            ' actionstr is a temp string for storing the current event in the loop
            actionstr = newtrack(timeindex)
            If actionstr.Substring(0, 1) = "9" Then

                Dim keyval As Short = Convert.ToInt16(actionstr.Substring(2, 2), 16)

                If Not activekeys.Contains(keyval) Then
                    activekeys.Add(keyval)
                    activekeysvel.Add(newtrackvel(timeindex))
                End If
            End If
        Next

        ' If there is at least 1 active key
        ' generate actionlist string for serial output and 
        ' call draw function for keyboard animation
        If activekeys.Count > 0 Then
            Dim actionlist As String = ""

            ' Sort active keys ascending
            activekeys.Sort()
            Dim lefthandlist As New List(Of Int16)
            lefthandlist = gendomain(activekeys(0), activekeys)

            handprocess(lefthandlist, activekeys, activekeysvel, True)

            Dim righthandlist As New List(Of Int16)


            If activekeys.Count > lefthandlist.Count Then
                righthandlist = gendomain(activekeys(lefthandlist.Count), activekeys)
                handprocess(righthandlist, activekeys, activekeysvel, False)
            End If
            
            Dim rht As Int32 = (minkeys(1) - maxspan)
            If righthandlist.Count = 0 Then
                If (lefthandlist(0) >= rht) And ((maxkeys(0) + maxspan) / 2) < rht Then
                    clearfingers()
                    handprocess(lefthandlist, activekeys, activekeysvel, False)
                End If
            End If

            If lefthandlist.Count > 0 Then
                minkeys(0) = lefthandlist(0)
                maxkeys(0) = lefthandlist(lefthandlist.Count - 1)
                If minkeysavg(0) > 0 Then
                    minkeysavg(0) = Math.Floor((minkeysavg(0) + lefthandlist(0)) / 2)
                Else
                    minkeysavg(0) = lefthandlist(0)
                End If
            End If

            If righthandlist.Count > 0 Then

                minkeys(1) = righthandlist(0)
                maxkeys(1) = righthandlist(righthandlist.Count - 1)

                If minkeysavg(1) > 0 Then
                    minkeysavg(1) = Math.Floor((minkeysavg(1) + righthandlist(0)) / 2)
                Else
                    minkeysavg(1) = righthandlist(0)
                End If
            Else

            End If

                For i As Integer = 0 To activekeyswhand.Count - 1

                    Dim finalvel As Int32 = activekeyswhandvel(i) + newtrackscalevel
                    If finalvel > 127 Then
                        finalvel = 127
                    End If

                    Dim fingerhex As String = Hex(activekeyswhandf(i))

                    If fingerhex.Length = 1 Then
                        fingerhex = "0" & fingerhex
                    End If

                    Dim keyhex As String = Hex(activekeyswhand(i))

                    If keyhex.Length = 1 Then
                        keyhex = "0" & keyhex
                    End If

                    ' Comment this out if you use the below value
                    actionlist += fingerhex

                    ' Uncomment out below to send finger and key with serial.
                    'actionlist += fingerhex & keyhex

                    ' If we are to play audio, create a new instance of our pianonote class
                    ' We pass in the key and the key velocity (volume)
                    ' DX handles the audio mixing for us.
                    If cbplayaudio.Checked = True Then
                        Dim ts As New pianonote(activekeyswhand(i), finalvel)
                        Dim t As New Thread(New ThreadStart(AddressOf ts.playnote))
                        t.Start()
                    End If
                Next

                ' Show serial output
                TextBox1.Text = actionlist

                ' Draw keyboard animation with most recent activekeys
                drawsub()


                ' Send actionlist to serial output
                ' 'K' is a delimiter for each active key which is always 2 digits long
                ' The number after the active key (the 3rd number) is the number of times
                ' the key was pressed in this tick position
                sendkeys_serial(actionlist & "+")

            nodraw = False

            If activekeys.Count > activekeyswhand.Count Then
                lkd.Visible = True
                lkd.Text = (activekeys.Count - activekeyswhand.Count) & " Key(s) Dropped (Finger Span)"
            ElseIf lkd.Visible = True Then
                lkd.Visible = False
            End If

            Else
                ' If no activekeys, draw a blank keyboard
                ' however, if there are multiple tickpos of no activity, 
                ' only draw blank keyboard once
                If nodraw = False Then

                    ' If sticky keys are ENABLED this will be skipped, which skips
                    ' rendering a blank keyboard until the next cycle of active keys.
                    ' Sticky keys makes it easier for someone to see what keys are being hit
                    If cbstickykeys.Checked = False Then
                        drawsub()
                    End If
                    nodraw = True
                End If

            End If

        activekeys.Clear()
        activekeysvel.Clear()
        activekeyswhand.Clear()
        activekeyswhandf.Clear()
        activekeyswhandfc.Clear()
        activekeyswhandh.Clear()
        tickpos += 1
    End Sub

    ' This function will split activekeys list into a domain of all keys possible for a single
    ' hand to hit based on the first key passed and other criteria for the range
    Private Function gendomain(ByVal firstkey As Int16, ByVal activekeys As List(Of Int16))

        Dim maxkeyval As Int16 = (firstkey + (Math.Ceiling(maxdistance / widthwhitekey)))

        Dim domainlist As New List(Of Int16)

        Dim curindex As Int32 = activekeys.IndexOf(firstkey)
        Dim maxindex As Int32 = curindex + 4
        While curindex <= maxindex And activekeys(curindex) <= maxkeyval
            domainlist.Add(activekeys(curindex))
            curindex += 1

            If curindex > (activekeys.Count - 1) Then
                Exit While
            End If

        End While

        Return domainlist
    End Function

    Private Function calckeydist(ByVal startkey As Int16, ByVal endkey As Int16)
        Dim returnval As Double = 0.0

        While startkey <= endkey

            ' If last key is black, then add only black key distance
            If startkey = endkey And blackkeypos.Contains(startkey) Then
                returnval += widthblackkey
            ElseIf Not blackkeypos.Contains(startkey) Then
                returnval += widthwhitekey
            End If

            startkey += 1
        End While

        Return returnval
    End Function

    Private Function findfingerpos(ByVal currentfinger As Int16, ByVal previouskey As Int16, ByVal currentkey As Int16)
        Dim returnval As Int16 = currentfinger

        If previouskey = 0 Then
            previouskey = currentkey
        End If

        ' If finger can reach a fraction of key width, then the finger can play the key
        Dim reachdist As Double = calckeydist(previouskey, currentkey)
        returnval = currentfinger + (Math.Ceiling(reachdist / maxfingerdistance))
        Return returnval
    End Function

    Private Sub genrect(ByVal rectnum As Int16, ByVal keywidth As Int16, ByVal keyheight As Int16, ByVal keyspacing As Int32, ByVal iswhitekeys As Boolean)

        ' Setup brushes and colors
        Dim myBrush As System.Drawing.SolidBrush
        Dim myBrush2 As System.Drawing.SolidBrush
        Dim myBrush3 As System.Drawing.SolidBrush
        Dim myBrush4 As System.Drawing.SolidBrush
        Dim myBrush5 As System.Drawing.SolidBrush

        myBrush = New System.Drawing.SolidBrush(System.Drawing.Color.White)
        myBrush2 = New System.Drawing.SolidBrush(System.Drawing.Color.Black)
        myBrush3 = New System.Drawing.SolidBrush(System.Drawing.Color.DimGray)
        myBrush4 = New System.Drawing.SolidBrush(System.Drawing.Color.Red)
        myBrush5 = New System.Drawing.SolidBrush(System.Drawing.Color.Yellow)

        ' New graphics object
        Dim formGraphics As System.Drawing.Graphics = Me.CreateGraphics()

        ' Y coord. location
        Dim yval As Int32 = TextBox1.Location.Y + TextBox1.Size.Height + 15

        ' If rectnum = 0, it is to draw the black/gray background
        ' Otherwise, proceed to keyboard generation
        ' All keys in active key list are red for right and yellow for left
        If rectnum <> 0 Then

            Dim startloop As Int16 = 21
            Dim offsetc As Int16 = 1

            ' Loop for white keys
            ' Any key not a black key is a white key
            While startloop <= totalkeys

                If Not blackkeypos.Contains(startloop) Then
                    Dim xval As Int32 = ((keywidth * offsetc) + (2 * offsetc))
                    If activekeyswhand.Contains(startloop) Then

                        Dim leftorright As String = activekeyswhandh(activekeyswhand.IndexOf(startloop))

                        If leftorright = "L" Then
                            formGraphics.FillRectangle(myBrush5, New Rectangle(xval, yval, keywidth, keyheight))
                        Else
                            formGraphics.FillRectangle(myBrush4, New Rectangle(xval, yval, keywidth, keyheight))
                        End If

                        drawkey(startloop, xval, yval, keywidth, keyheight)

                    Else
                        formGraphics.FillRectangle(myBrush, New Rectangle(xval, yval, keywidth, keyheight))
                    End If
                    offsetc += 1
                End If
                startloop += 1
            End While

            startloop = 21
            offsetc = 1

            ' Loop for black keys, look for keys in black key list
            While startloop <= totalkeys
                If Not blackkeypos.Contains(startloop) Then
                    offsetc += 1
                Else
                    Dim xval As Int32 = ((keywidth * (offsetc)) + (2 * (offsetc)))

                    If activekeyswhand.Contains(startloop) Then

                        Dim leftorright As String = activekeyswhandh(activekeyswhand.IndexOf(startloop))

                        If leftorright = "L" Then
                            formGraphics.FillRectangle(myBrush5, New Rectangle(xval - (keywidth / 2), yval, keywidth, keyheight / 2))
                        Else
                            formGraphics.FillRectangle(myBrush4, New Rectangle(xval - (keywidth / 2), yval, keywidth, keyheight / 2))
                        End If

                        drawkey(startloop, xval - (keywidth / 2), yval, keywidth, keyheight / 2)

                    Else
                        formGraphics.FillRectangle(myBrush2, New Rectangle(xval - (keywidth / 2), yval, keywidth, keyheight / 2))
                    End If
                End If
                startloop += 1
            End While

        Else

            ' Black/gray background drawing
            formGraphics.Clear(Color.WhiteSmoke)
            formGraphics.FillRectangle(myBrush3, New Rectangle(0, yval - 5, keywidth, keyheight + 6))
            formGraphics.FillRectangle(myBrush2, New Rectangle(0, yval - 5, keywidth, 5))
        End If

        ' Dispose of graphic objects
        myBrush.Dispose()
        myBrush2.Dispose()
        myBrush3.Dispose()
        myBrush4.Dispose()
        formGraphics.Dispose()
    End Sub

    ' Sub to define basic parameters for and execute drawing function above
    Private Sub drawsub()
        Dim wkeywidth As Int32 = (Me.Width - (56 * 2)) / 56
        Dim keyspacing As Int16 = 1
        Dim wkeyheight As Int32 = Me.Width / 10

        Dim whitekeyloop = 1
        Dim blackkeyloop = 1


        genrect(0, Me.Width - 2, wkeyheight, keyspacing, True)

        genrect(1, wkeywidth, wkeyheight, keyspacing, True)
    End Sub

    Private Sub drawkey(ByVal keystring As String, ByVal xval As Int32, ByVal yval As Int32, ByVal keywidth As Int16, ByVal keyheight As Int16)
        Dim myBrush2 As New System.Drawing.SolidBrush(System.Drawing.Color.Black)
        Dim myBrush3 As New System.Drawing.SolidBrush(System.Drawing.Color.White)
        Dim formGraphics As System.Drawing.Graphics = Me.CreateGraphics()
        For index As Integer = 0 To activekeyswhand.Count - 1
            Dim strsize As Single = keywidth - (keywidth / 4)

            If activekeyswhand(index) = keystring Then

                Dim keyval As Int16 = activekeyswhandf(index)
                Dim drawFont As New Font("Arial", strsize)

                If keyval > 5 Then
                    keyval -= 5
                ElseIf keyval <= 5 Then
                    keyval = Math.Abs(keyval - 6)
                End If

                yval = yval + keyheight - (Math.Floor(strsize) * 2)

                If keyval <= 5 Then
                    formGraphics.DrawString(Convert.ToString(keyval), drawFont, myBrush2, New Point(xval, yval))
                Else
                    formGraphics.DrawString(Convert.ToString(keyval), drawFont, myBrush3, New Point(xval, yval))
                End If
                drawFont.Dispose()
            End If
        Next

        myBrush2.Dispose()
        formGraphics.Dispose()
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)

        drawsub()
    End Sub

    Private Sub closingsub() Handles MyBase.FormClosing
        ticktimer.Enabled = False
    End Sub


    ' Send data to serial link
    Private Sub sendkeys_serial(ByVal keystr As String)

        If cbsendserial.Checked = True Then
            serialsend(keystr)
        End If

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If midifileopen.ShowDialog <> Windows.Forms.DialogResult.Cancel Then
            loadedfilename = midifileopen.FileName
            readfilename()
        End If
    End Sub

    Private Sub midihaptic_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)

        loadedfilename = files(0)
        readfilename()
    End Sub

    Private Sub midihaptic_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub readfilename()
        ticktimer.Enabled = False
        resetdata()
        updatestatus(1)
        tbplayspeed.Value = 1
        readheader()
        readtracks(18, 1)
        striptracklen()
        readevents()
        setticktime()
        updatestatus(2)
    End Sub

    Private Sub resetdata()
        'loadedfilename = ""
        timeheader = ""
        tickpos = 0
        chunkc.Value = 1
        fformat = -1
        ftrackc = 1
        ftdiv = 0
        ftdivtype = 0
        bpm = 120
        mpb = 500000
        ticktime = 8333.33
        fpsval = 24
        badfile = False
        tracka.Clear()
        compressedtrack.Clear()
        compressedtracktimes.Clear()
        compressedtrackvel.Clear()
        activekeys.Clear()
        activekeyswhand.Clear()
        activekeyswhandf.Clear()
        activekeyswhandh.Clear()
        activekeyswhandfc.Clear()
        activekeysvel.Clear()
        activekeyswhandvel.Clear()
        newtrackvel.Clear()
        newtrackscalevel = 0
        runningstatus = 0
        runningstatusactive = False
        newtrack.Clear()
        newtracktimes.Clear()
        keycount.Clear()
    End Sub

    Private Sub updatestatus(ByVal statuscode As Int16)

        ' 0 = no file loaded
        ' 1 = processing file
        ' 2 = file loaded
        If statuscode = 0 Then
            floadstatus.Text = "No File Loaded"
            btnplay.Enabled = False
            btnpause.Enabled = False
            btnstop.Enabled = False
        ElseIf statuscode = 1 Then
            floadstatus.Text = "Processing File..."
            btnplay.Enabled = False
            btnstop.Enabled = False
            btnpause.Enabled = False
        ElseIf statuscode = 2 Then
            floadstatus.Text = "File Loaded: " & loadedfilename
            btnplay.Enabled = True
            btnstop.Enabled = True
            btnpause.Enabled = True
            chunkc.Enabled = True
            chunkselect.Enabled = True
            chunkselect.SelectedIndex = 0
        End If
    End Sub


    Private Sub calcfingerspan()
        ' Max key span of person, user configurable
        maxspan = Convert.ToInt16(tmaxspan.Text)
        maxdistance = Math.Floor((maxspan * widthwhitekey) + 0.1)
        maxfingerdistance = maxdistance * individualratio
    End Sub


    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        calcfingerspan()
    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnpause.Click
        ticktimer.Enabled = False
    End Sub

    Private Function timefromseconds(ByVal secondsval As Int32)
        Dim timespanobj As TimeSpan = TimeSpan.FromSeconds(secondsval)
        Return timespanobj.Minutes.ToString().PadLeft(2, "0") & ":" & timespanobj.Seconds.ToString().PadLeft(2, "0")
    End Function

    Private Sub chunkc_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chunkc.Scroll
        chunkcval.Text = chunkc.Value

        chunkselect.Items.Clear()

        Dim startval As Int16 = 1

        While startval <= chunkc.Value
            chunkselect.Items.Add(startval)
            startval += 1
        End While

        chunkselect.SelectedIndex = 0
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chunkselect.SelectedIndexChanged
        '
        Dim chunksteps As Int32 = Math.Floor(newtracktimes.Max() / chunkc.Value)
        Dim selectedchunk As Int16 = chunkselect.SelectedIndex + 1
        mintickval = (selectedchunk - 1) * chunksteps
        maxtickval = selectedchunk * chunksteps

        mintval.Text = mintickval
        maxtval.Text = maxtickval
        tickpos = mintickval

    End Sub


    Private Sub tbplayspeed_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbplayspeed.Scroll
        updatespeedtime()
    End Sub

    Private Sub updatespeedtime()
        Dim actualspeedval As Decimal = tbplayspeed.Value
        Dim showspeed As String = tbplayspeed.Value

        If actualspeedval > 1 Then
            showspeed = (1 + ((actualspeedval * 1) / 10))
            actualspeedval = 1 / (1 + ((actualspeedval * 1) / 10))
        ElseIf actualspeedval < 1 Then

            ' Never have a 0 speed, wish trackbar would let you exclude a number from range
            If actualspeedval = 0 Then
                showspeed = "1 / 2"
                actualspeedval = 2
            Else
                showspeed = "1 / " & (Math.Abs(actualspeedval) + 2)
                actualspeedval = (Math.Abs(actualspeedval) + 2)
            End If
        End If

        lplayspeed.Text = " x " & showspeed
        ticktimer.Interval = compressedtime * (actualspeedval)
        ltimet.Text = "Time (" & timefromseconds(maxtickval * (ticktimer.Interval / 1000)) & "):"
    End Sub


    ' Serial port data

    Private myComPort As New SerialPort

    Private Sub serialopen()

        Try

            If Not myComPort.IsOpen And serialporttxt.SelectedItem <> vbNullString Then
                myComPort.PortName = serialporttxt.SelectedItem

                myComPort.BaudRate = 9600
                myComPort.Parity = Parity.None
                myComPort.DataBits = 8
                myComPort.StopBits = StopBits.One
                myComPort.Handshake = Handshake.None

                myComPort.ReadTimeout = 3000
                myComPort.WriteTimeout = 5000

                myComPort.Open()

            End If

        Catch ex As InvalidOperationException
            MessageBox.Show(ex.Message)

        Catch ex As UnauthorizedAccessException
            MessageBox.Show(ex.Message)

        Catch ex As System.IO.IOException
            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub serialclose()

        Using myComPort

            If (Not (myComPort Is Nothing)) Then

                ' The COM port exists.

                If myComPort.IsOpen Then

                    ' Wait for the transmit buffer to empty.

                    Do While (myComPort.BytesToWrite > 0)
                    Loop

                End If
            End If

        End Using

    End Sub

    Private Sub serialsend(ByVal command As String)

        'Dim response As String

        Try

            myComPort.Write(command)
            ' response = myComPort.ReadLine
        Catch ex As Exception
            MsgBox("Serial connection error, check your serial settings. Serial output disabled automatically for this session. Error: " & ex.Message)
            cbsendserial.Checked = False
            ' MessageBox.Show()
        End Try

    End Sub


End Class

Public Class pianonote
    Private keyval As Int16
    Private keyvelocity As Int16

    ' Location of notes sampling
    Private noteslocation As String = Path.Combine(Application.StartupPath, "notes\")

    ' The constructor obtains the state information.
    Public Sub New(ByVal keyv As Int16, ByVal keyvel As Int16)
        keyval = keyv + 20
        keyvelocity = keyvel
    End Sub

    ' May replace collection of individual samples with a single
    ' sample and do pitch shifting instead. Need to research fast realtime
    ' pitch shifting algorithms that don't alter the samples time domain

    ' Function below simply looks for the note mp3 file by the passed value
    ' and plays sound with DirectSound's Audio class. This is very simple
    ' and seems to support overlapping audio streams automatically
    ' (AKA, DirectSound handles the mixing within it's own library).
    Public Sub playnote()
        keyval -= 20

        If keyval >= 21 And keyval <= 108 Then
            Dim notefile As String = noteslocation & keyval & ".mp3"
            Dim noteobj As New Audio(notefile, False)

            'Dim scalesub As Int32 = Math.Floor(Math.Abs((keyvelocity - 127) / 4))
            'keyvelocity -= scalesub

            If keyvelocity < 0 Then
                keyvelocity = 0
            End If

            ' Scales from a 0-127 volume range to a 10,000-0 volume range
            Dim scaledvel As Int32 = Math.Floor((keyvelocity / 127) * (10000)) - 10000
            noteobj.Volume = scaledvel

            ' Play audio file
            noteobj.Play()

            ' Sleep for a second or two and then dispose
            ' Sleep is needed to keep DX audio playing until end of piano sample
            ' If you dispose of thread right away it will kill the audio sample with it...
            ' Piano samples should only be 2 seconds max, but give 500 ms extra for safety 
            Threading.Thread.Sleep(2500)
            noteobj.Dispose()
        End If
    End Sub
End Class
