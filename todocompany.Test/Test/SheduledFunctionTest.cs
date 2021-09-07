using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using todocompany.Functions.Functions;
using todocompany.Test.Helpers;
using Xunit;

namespace todocompany.Test.Test
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            //Arrange
            MockCloudTableEntrys mockEntrys = new MockCloudTableEntrys(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            //Act
            ScheduledFunction.Run(null, mockEntrys, logger);
            string message = logger.Logs[0];

            //Asert
            Assert.Contains("Deleting completed", message);
        }
    }
}
