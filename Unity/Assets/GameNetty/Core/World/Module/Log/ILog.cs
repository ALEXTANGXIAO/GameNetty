﻿using System;

namespace ET
{
    public interface ILog
    {
        void Trace(string message);
        
        void Warning(string message);
        
        void Info(string message);
        
        void Debug(string message);
        
        void Error(string message);
        
        void Error(Exception e);
    }
}