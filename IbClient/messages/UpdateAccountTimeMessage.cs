﻿/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

namespace IBSampleApp.messages
{
    public class UpdateAccountTimeMessage
    {
        public UpdateAccountTimeMessage(string timestamp)
        {
            Timestamp = timestamp;
        }

        public string Timestamp { get; set; }
    }
}
