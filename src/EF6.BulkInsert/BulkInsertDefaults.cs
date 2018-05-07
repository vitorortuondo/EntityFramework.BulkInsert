﻿namespace EF6.BulkInsert
{
    public static class BulkInsertDefaults
    {
        public static int BatchSize = 500;
        public static BulkCopyOptions BulkCopyOptions = BulkCopyOptions.Default;
        public static int TimeOut = 30;
        public static int NotifyAfter = 1000;
    }
}
