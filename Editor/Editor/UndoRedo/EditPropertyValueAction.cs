﻿using System;

namespace WindEditor
{
    public class WEditPropertyValueAction : WUndoCommand
    {
        private Action m_undoAction;
        private Action m_redoAction;
        private Action m_onPropertyChanged;

        public WEditPropertyValueAction(Action onUndo, Action onRedo, Action onPropertyChanged, WUndoCommand parent = null) : base("Value", parent)
        {
            if (onUndo == null)
                throw new ArgumentNullException("onUndo", "Undo callback cannot be null.");
            if (onRedo == null)
                throw new ArgumentNullException("onRedo", "Redo callback cannot be null.");
            if (onPropertyChanged == null)
                throw new ArgumentNullException("onPropertyChanged", "On Property Changed callback cannot be null.");

            m_undoAction = onUndo;
            m_redoAction = onRedo;
            m_onPropertyChanged = onPropertyChanged;
        }

        public override bool MergeWith(WUndoCommand withAction)
        {
            return false;
        }

        public override void Redo()
        {
            base.Redo();
            m_redoAction.Invoke();
            m_onPropertyChanged.Invoke();
        }

        public override void Undo()
        {
            base.Undo();
            m_undoAction.Invoke();
            m_onPropertyChanged.Invoke();
        }
    }
}
