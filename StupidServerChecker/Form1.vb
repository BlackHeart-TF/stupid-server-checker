Imports System
Imports System.ComponentModel
Imports System.Threading
Imports System.Windows.Forms
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.InteropServices
Public Class Form1
    Dim MajorV As Integer
    Dim MinorV As Integer
    Dim mapleInst As New Process
    Dim msdir As String
    Dim last As Boolean = True
    Dim current As Boolean
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindow(
     ByVal lpClassName As String,
     ByVal lpWindowName As String) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer

    End Function
    Private Const WM_SYSCOMMAND = &H112
    Private Const SC_CLOSE = &HF060

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        msdir = findmsdir()
        mapleInst.StartInfo.FileName = msdir & "\Maplestory.exe"
        mapleInst.StartInfo.Arguments = "GameLaunching"
        mapleInst.StartInfo.WorkingDirectory = msdir
        If msdir = "null" Then
            CheckBox1.Checked = False
            CheckBox1.Visible = False
        Else
            CheckBox1.Checked = True
        End If

        CheckBox2.Checked = True
        Timer1.Start()
    End Sub



    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Not BackgroundWorker1.IsBusy Then
            BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub

    Private Sub startmaple()
        mapleInst.Start()
        Threading.Thread.Sleep(4000)
        clickplay()
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        current = isMapleUp()
    End Sub

    Private Sub bgw_done(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If e.Error IsNot Nothing Then
            Label2.Text = "Error: " & e.Error.Message
            GoTo endl

        End If
        If current = True Then
            Label2.Text = "Online v" + MajorV.ToString + "." + MinorV.ToString
        Else
            Label2.Text = "Offline"
        End If
        If (current = Not last) And (current = True) Then
            If CheckBox2.Checked = True Then
                My.Computer.Audio.Play(My.Resources.alarm, AudioPlayMode.Background)
            End If
            If CheckBox1.Checked = True Then
                startmaple()
            End If
        End If
        last = current
endl:

    End Sub

    Private Function isMapleUp()
        'Try
        '    Dim loginserv As TcpClient = New TcpClient("8.31.99.141", 8484)
        '    current = loginserv.Connected
        '    loginserv.Close()
        '    Return current
        'Catch
        '    Return False
        'End Try
        Try
            Dim serverStream As NetworkStream
            Dim serverIP As IPAddress = IPAddress.Parse("8.31.99.141")
            Dim clientSocket As TcpClient = New TcpClient()
            clientSocket.Connect(serverIP, 8484)
            serverStream = clientSocket.GetStream()
            Dim inStream(100000) As Byte
            serverStream.Read(inStream, 0, clientSocket.ReceiveBufferSize)
            MajorV = BitConverter.ToInt16(inStream, 2)
            MinorV = Convert.ToInt16(getStingFromByteA(inStream, 4))
            current = clientSocket.Connected
            clientSocket.Close()
            Return current
        Catch
            Return False
        End Try



    End Function
    Private Function getStingFromByteA(byteArray As Byte(), Startbyte As Integer)
        Dim characters As Integer = byteArray(Startbyte)
        Dim output As String = String.Empty
        For i As Integer = Startbyte + 2 To Startbyte + 1 + characters Step +1
            output = output + Convert.ToChar(byteArray(i))
        Next
        Return output
    End Function
    Private Function findmsdir() As String
        Dim dir As String = vbNull
        Dim pass As Boolean = False

        dir = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Wizet\MapleStory", "ExecPath", "C:\Nexon\Maplestory")
        If My.Computer.FileSystem.FileExists(dir & "\maplestory.exe") Then
            'MessageBox.Show("1 " & dir)
            pass = True
            GoTo found
        End If
        dir = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\MicroSoft\Windows\CurrentVersion\Uninstall\MapleStory", "InstallLocation", "C:\Nexon\Maplestory")
        If My.Computer.FileSystem.FileExists(dir & "\maplestory.exe") Then
            'MessageBox.Show("2 " & dir)

            pass = True
            GoTo found
        End If
        dir = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\NoireLoader", "maplestoryfolder", vbNull)
        If My.Computer.FileSystem.FileExists(dir & "\maplestory.exe") Then
            'MessageBox.Show("2 " & dir)

            pass = True
            GoTo found
        End If
        dir = "C:\Nexon\Maplestory"
        If My.Computer.FileSystem.FileExists(dir & "\maplestory.exe") Then
            'MessageBox.Show("3 " & dir)
            pass = True
            GoTo found
        End If
found:
        If pass Then
            'MessageBox.Show("returning: " & dir)
            Return dir
        Else

            MessageBox.Show("MapleStory.exe not found." + vbNewLine + "Autostart disabled.")
            Return "null"
        End If

    End Function

    Private Sub clickplay()
        Threading.Thread.Sleep(200)
        Dim i As Long = FindWindow("StartUpDlgClass", "MapleStory")
        'MessageBox.Show(i.ToString("X"))
        'PostMessage(i, WM_CLOSE, CLng(0), CLng(0))
        SendMessage(i, WM_SYSCOMMAND, SC_CLOSE, 0)
    End Sub
End Class
