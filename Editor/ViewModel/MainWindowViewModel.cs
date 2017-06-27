﻿using OpenTK;
using System.ComponentModel;
using System.Windows.Input;
using System;

namespace WindEditor.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WWindEditor WindEditor { get { return m_editor; } }
        public ICommand SetDataRootCommand { get { return new RelayCommand(x => OnUserSetDataRoot()); } }
        public ICommand ExitApplicationCommand { get { return new RelayCommand(x => OnUserRequestApplicationExit()); } }

        private WWindEditor m_editor;
        private GLControl m_glControl;
        private bool m_editorIsShuttingDown;

        public MainWindowViewModel()
        {
            App.Current.MainWindow.Closing += OnMainWindowClosed;
        }

        internal void OnMainEditorWindowLoaded(GLControl glControl)
        {
            m_glControl = glControl;

            // Delay the creation of the editor until the UI is created, so that we can fire off GL commands immediately in the editor.
            m_editor = new WWindEditor();
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("WindEditor"));

            // Set up the Editor Tick Loop
            System.Windows.Forms.Timer editorTickTimer = new System.Windows.Forms.Timer();
            editorTickTimer.Interval = 16; //ms
            editorTickTimer.Tick += (o, args) =>
            {
                DoApplicationTick();
            };
            editorTickTimer.Enabled = true;
        }

        private void DoApplicationTick()
        {
            if (m_editorIsShuttingDown)
                return;

            // Poll the mouse at a high resolution
            System.Drawing.Point mousePos = m_glControl.PointToClient(System.Windows.Forms.Cursor.Position);

            mousePos.X = WMath.Clamp(mousePos.X, 0, m_glControl.Width);
            mousePos.Y = WMath.Clamp(mousePos.Y, 0, m_glControl.Height);
            WInput.SetMousePosition(new Vector2(mousePos.X, mousePos.Y));

            m_editor.ProcessTick();
            WInput.Internal_UpdateInputState();

            m_glControl.SwapBuffers();
        }

        private void OnUserSetDataRoot()
        {
            // Violate dat MVVM.
            WindEditor.View.OptionsMenu optionsMenu = new View.OptionsMenu();
            optionsMenu.Show();
        }

        private void OnUserRequestApplicationExit()
        {
            // This attempts to close the application, which invokes the normal window close events.
            App.Current.MainWindow.Close();
        }

        private void OnMainWindowClosed(object sender, CancelEventArgs e)
        {
            /*if(someChangesExist)
                if(UserWantsToSave)
                    e.Cancel = true;*/

            m_editorIsShuttingDown = true;
            m_editor.Shutdown();
        }
    }
}
