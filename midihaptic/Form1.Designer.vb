<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class midihaptic
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(midihaptic))
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ticktimer = New System.Timers.Timer()
        Me.btnplay = New System.Windows.Forms.Button()
        Me.btnstop = New System.Windows.Forms.Button()
        Me.ticklabel = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.midifileopen = New System.Windows.Forms.OpenFileDialog()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.floadstatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tmaxspan = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.btnpause = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.serialporttxt = New System.Windows.Forms.ComboBox()
        Me.cbplayaudio = New System.Windows.Forms.CheckBox()
        Me.ltimet = New System.Windows.Forms.Label()
        Me.ltime = New System.Windows.Forms.Label()
        Me.chunkc = New System.Windows.Forms.TrackBar()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.chunkcval = New System.Windows.Forms.Label()
        Me.chunkselect = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.mintval = New System.Windows.Forms.Label()
        Me.maxtval = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.cbrepeat = New System.Windows.Forms.CheckBox()
        Me.tbplayspeed = New System.Windows.Forms.TrackBar()
        Me.lplayspeed = New System.Windows.Forms.Label()
        Me.cbstickykeys = New System.Windows.Forms.CheckBox()
        Me.cbsendserial = New System.Windows.Forms.CheckBox()
        Me.lkd = New System.Windows.Forms.Label()
        CType(Me.ticktimer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.chunkc, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbplayspeed, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Location = New System.Drawing.Point(12, 195)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(622, 32)
        Me.TextBox1.TabIndex = 0
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(17, 59)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(47, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Debug"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ticktimer
        '
        Me.ticktimer.Interval = 4.59R
        Me.ticktimer.SynchronizingObject = Me
        '
        'btnplay
        '
        Me.btnplay.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnplay.Location = New System.Drawing.Point(443, 4)
        Me.btnplay.Name = "btnplay"
        Me.btnplay.Size = New System.Drawing.Size(75, 23)
        Me.btnplay.TabIndex = 2
        Me.btnplay.Text = "Play"
        Me.btnplay.UseVisualStyleBackColor = True
        '
        'btnstop
        '
        Me.btnstop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnstop.Location = New System.Drawing.Point(593, 4)
        Me.btnstop.Name = "btnstop"
        Me.btnstop.Size = New System.Drawing.Size(41, 23)
        Me.btnstop.TabIndex = 3
        Me.btnstop.Text = "Stop"
        Me.btnstop.UseVisualStyleBackColor = True
        '
        'ticklabel
        '
        Me.ticklabel.AutoSize = True
        Me.ticklabel.Location = New System.Drawing.Point(92, 86)
        Me.ticklabel.Name = "ticklabel"
        Me.ticklabel.Size = New System.Drawing.Size(13, 13)
        Me.ticklabel.TabIndex = 4
        Me.ticklabel.Text = "0"
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(176, 36)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(88, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Playback Speed:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(13, 85)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(78, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Track Position:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.CornflowerBlue
        Me.Label4.Location = New System.Drawing.Point(6, 4)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(150, 31)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "MIDIHaptic"
        '
        'Button4
        '
        Me.Button4.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button4.Location = New System.Drawing.Point(534, 370)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(100, 23)
        Me.Button4.TabIndex = 9
        Me.Button4.Text = "Load MIDI File"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.floadstatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 405)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(646, 22)
        Me.StatusStrip1.TabIndex = 10
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'floadstatus
        '
        Me.floadstatus.Name = "floadstatus"
        Me.floadstatus.Size = New System.Drawing.Size(631, 17)
        Me.floadstatus.Spring = True
        Me.floadstatus.Text = "No File Loaded"
        '
        'tmaxspan
        '
        Me.tmaxspan.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.tmaxspan.Location = New System.Drawing.Point(148, 373)
        Me.tmaxspan.Name = "tmaxspan"
        Me.tmaxspan.Size = New System.Drawing.Size(36, 20)
        Me.tmaxspan.TabIndex = 12
        Me.tmaxspan.Text = "8"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(7, 376)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(142, 13)
        Me.Label5.TabIndex = 13
        Me.Label5.Text = "Max White Key Finger Span:"
        '
        'Button2
        '
        Me.Button2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Button2.Location = New System.Drawing.Point(188, 370)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(61, 23)
        Me.Button2.TabIndex = 14
        Me.Button2.Text = "Change"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'btnpause
        '
        Me.btnpause.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnpause.Location = New System.Drawing.Point(524, 4)
        Me.btnpause.Name = "btnpause"
        Me.btnpause.Size = New System.Drawing.Size(63, 23)
        Me.btnpause.TabIndex = 15
        Me.btnpause.Text = "Pause"
        Me.btnpause.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(9, 179)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(71, 13)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Serial Output:"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(460, 174)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(58, 13)
        Me.Label1.TabIndex = 19
        Me.Label1.Text = "Serial Port:"
        '
        'serialporttxt
        '
        Me.serialporttxt.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.serialporttxt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.serialporttxt.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.serialporttxt.FormattingEnabled = True
        Me.serialporttxt.Location = New System.Drawing.Point(524, 171)
        Me.serialporttxt.Name = "serialporttxt"
        Me.serialporttxt.Size = New System.Drawing.Size(111, 21)
        Me.serialporttxt.TabIndex = 18
        '
        'cbplayaudio
        '
        Me.cbplayaudio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbplayaudio.AutoSize = True
        Me.cbplayaudio.Checked = True
        Me.cbplayaudio.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbplayaudio.Location = New System.Drawing.Point(284, 8)
        Me.cbplayaudio.Name = "cbplayaudio"
        Me.cbplayaudio.Size = New System.Drawing.Size(153, 17)
        Me.cbplayaudio.TabIndex = 20
        Me.cbplayaudio.Text = "Play Audio (may cause lag)"
        Me.cbplayaudio.UseVisualStyleBackColor = True
        '
        'ltimet
        '
        Me.ltimet.AutoSize = True
        Me.ltimet.Location = New System.Drawing.Point(14, 39)
        Me.ltimet.Name = "ltimet"
        Me.ltimet.Size = New System.Drawing.Size(69, 13)
        Me.ltimet.TabIndex = 21
        Me.ltimet.Text = "Time (00.00):"
        '
        'ltime
        '
        Me.ltime.AutoSize = True
        Me.ltime.Location = New System.Drawing.Point(84, 39)
        Me.ltime.Name = "ltime"
        Me.ltime.Size = New System.Drawing.Size(34, 13)
        Me.ltime.TabIndex = 22
        Me.ltime.Text = "00:00"
        '
        'chunkc
        '
        Me.chunkc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chunkc.Enabled = False
        Me.chunkc.Location = New System.Drawing.Point(12, 123)
        Me.chunkc.Maximum = 100
        Me.chunkc.Minimum = 1
        Me.chunkc.Name = "chunkc"
        Me.chunkc.Size = New System.Drawing.Size(623, 45)
        Me.chunkc.TabIndex = 23
        Me.chunkc.Value = 1
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(14, 111)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(72, 13)
        Me.Label7.TabIndex = 24
        Me.Label7.Text = "Chunk Count:"
        '
        'chunkcval
        '
        Me.chunkcval.AutoSize = True
        Me.chunkcval.Location = New System.Drawing.Point(83, 111)
        Me.chunkcval.Name = "chunkcval"
        Me.chunkcval.Size = New System.Drawing.Size(13, 13)
        Me.chunkcval.TabIndex = 25
        Me.chunkcval.Text = "1"
        '
        'chunkselect
        '
        Me.chunkselect.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chunkselect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.chunkselect.Enabled = False
        Me.chunkselect.FormattingEnabled = True
        Me.chunkselect.Items.AddRange(New Object() {"1"})
        Me.chunkselect.Location = New System.Drawing.Point(570, 98)
        Me.chunkselect.Name = "chunkselect"
        Me.chunkselect.Size = New System.Drawing.Size(64, 21)
        Me.chunkselect.TabIndex = 26
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(478, 101)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(86, 13)
        Me.Label8.TabIndex = 27
        Me.Label8.Text = "Selected Chunk:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(14, 98)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(51, 13)
        Me.Label9.TabIndex = 28
        Me.Label9.Text = "Min. Pos:"
        '
        'mintval
        '
        Me.mintval.AutoSize = True
        Me.mintval.Location = New System.Drawing.Point(64, 98)
        Me.mintval.Name = "mintval"
        Me.mintval.Size = New System.Drawing.Size(13, 13)
        Me.mintval.TabIndex = 29
        Me.mintval.Text = "0"
        '
        'maxtval
        '
        Me.maxtval.AutoSize = True
        Me.maxtval.Location = New System.Drawing.Point(169, 98)
        Me.maxtval.Name = "maxtval"
        Me.maxtval.Size = New System.Drawing.Size(13, 13)
        Me.maxtval.TabIndex = 31
        Me.maxtval.Text = "0"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(119, 98)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(54, 13)
        Me.Label12.TabIndex = 30
        Me.Label12.Text = "Max. Pos:"
        '
        'cbrepeat
        '
        Me.cbrepeat.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbrepeat.AutoSize = True
        Me.cbrepeat.Checked = True
        Me.cbrepeat.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbrepeat.Location = New System.Drawing.Point(203, 8)
        Me.cbrepeat.Name = "cbrepeat"
        Me.cbrepeat.Size = New System.Drawing.Size(61, 17)
        Me.cbrepeat.TabIndex = 32
        Me.cbrepeat.Text = "Repeat"
        Me.cbrepeat.UseVisualStyleBackColor = True
        '
        'tbplayspeed
        '
        Me.tbplayspeed.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbplayspeed.Location = New System.Drawing.Point(172, 50)
        Me.tbplayspeed.Maximum = 50
        Me.tbplayspeed.Minimum = -20
        Me.tbplayspeed.Name = "tbplayspeed"
        Me.tbplayspeed.Size = New System.Drawing.Size(462, 45)
        Me.tbplayspeed.TabIndex = 33
        Me.tbplayspeed.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.tbplayspeed.Value = 1
        '
        'lplayspeed
        '
        Me.lplayspeed.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lplayspeed.AutoSize = True
        Me.lplayspeed.Location = New System.Drawing.Point(263, 37)
        Me.lplayspeed.Name = "lplayspeed"
        Me.lplayspeed.Size = New System.Drawing.Size(24, 13)
        Me.lplayspeed.TabIndex = 34
        Me.lplayspeed.Text = " x 1"
        '
        'cbstickykeys
        '
        Me.cbstickykeys.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbstickykeys.AutoSize = True
        Me.cbstickykeys.Checked = True
        Me.cbstickykeys.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbstickykeys.Location = New System.Drawing.Point(10, 356)
        Me.cbstickykeys.Name = "cbstickykeys"
        Me.cbstickykeys.Size = New System.Drawing.Size(123, 17)
        Me.cbstickykeys.TabIndex = 35
        Me.cbstickykeys.Text = "Enable Sticky Keys?"
        Me.cbstickykeys.UseVisualStyleBackColor = True
        '
        'cbsendserial
        '
        Me.cbsendserial.AutoSize = True
        Me.cbsendserial.Location = New System.Drawing.Point(87, 178)
        Me.cbsendserial.Name = "cbsendserial"
        Me.cbsendserial.Size = New System.Drawing.Size(84, 17)
        Me.cbsendserial.TabIndex = 36
        Me.cbsendserial.Text = "Send serial?"
        Me.cbsendserial.UseVisualStyleBackColor = True
        '
        'lkd
        '
        Me.lkd.AutoSize = True
        Me.lkd.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lkd.ForeColor = System.Drawing.Color.Crimson
        Me.lkd.Location = New System.Drawing.Point(177, 174)
        Me.lkd.Name = "lkd"
        Me.lkd.Size = New System.Drawing.Size(118, 20)
        Me.lkd.TabIndex = 37
        Me.lkd.Text = "- Keys Dropped"
        Me.lkd.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.lkd.Visible = False
        '
        'midihaptic
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(646, 427)
        Me.Controls.Add(Me.lkd)
        Me.Controls.Add(Me.cbsendserial)
        Me.Controls.Add(Me.cbstickykeys)
        Me.Controls.Add(Me.lplayspeed)
        Me.Controls.Add(Me.tbplayspeed)
        Me.Controls.Add(Me.cbrepeat)
        Me.Controls.Add(Me.maxtval)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.mintval)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.chunkselect)
        Me.Controls.Add(Me.chunkcval)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.chunkc)
        Me.Controls.Add(Me.ltime)
        Me.Controls.Add(Me.ltimet)
        Me.Controls.Add(Me.cbplayaudio)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.serialporttxt)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.btnpause)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.tmaxspan)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ticklabel)
        Me.Controls.Add(Me.btnstop)
        Me.Controls.Add(Me.btnplay)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "midihaptic"
        Me.Text = "MIDIHaptic - Load and deconstruct MIDI Files for haptic feedback"
        CType(Me.ticktimer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        CType(Me.chunkc, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbplayspeed, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ticktimer As System.Timers.Timer
    Friend WithEvents btnstop As System.Windows.Forms.Button
    Friend WithEvents btnplay As System.Windows.Forms.Button
    Friend WithEvents ticklabel As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents midifileopen As System.Windows.Forms.OpenFileDialog
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents floadstatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents tmaxspan As System.Windows.Forms.TextBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents btnpause As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents serialporttxt As System.Windows.Forms.ComboBox
    Friend WithEvents cbplayaudio As System.Windows.Forms.CheckBox
    Friend WithEvents ltime As System.Windows.Forms.Label
    Friend WithEvents ltimet As System.Windows.Forms.Label
    Friend WithEvents chunkc As System.Windows.Forms.TrackBar
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents chunkcval As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents chunkselect As System.Windows.Forms.ComboBox
    Friend WithEvents maxtval As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents mintval As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents cbrepeat As System.Windows.Forms.CheckBox
    Friend WithEvents tbplayspeed As System.Windows.Forms.TrackBar
    Friend WithEvents lplayspeed As System.Windows.Forms.Label
    Friend WithEvents cbstickykeys As System.Windows.Forms.CheckBox
    Friend WithEvents cbsendserial As System.Windows.Forms.CheckBox
    Friend WithEvents lkd As System.Windows.Forms.Label

End Class
