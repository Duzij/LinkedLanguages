﻿
using System;

namespace LinkedLanguages.BL.User
{
    public interface IAppUserProvider
    {
        public Guid GetUserId();
        string GetUserKnownLanguageCode();
        string GetUserUnknownLanguageCode();
    }
}
