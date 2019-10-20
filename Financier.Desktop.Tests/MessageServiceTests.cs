using System;
using Financier.Desktop.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Financier.Desktop.Tests
{
    [TestClass]
    public class MessageServiceTests
    {
        private class TestMessage
        {
            internal TestMessage(string text)
            {
                Text = text;
            }

            internal string Text { get; private set; }
        };

        [TestMethod]
        public void TestRegister()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            int numCalls = 0;
            Action<TestMessage> testAction = (testMessage) =>
            {
                ++numCalls;
            };

            var messageService = new MessageService(loggerFactory);
            messageService.Register<TestMessage>(testAction);

            Assert.AreEqual(0, numCalls);
        }

        [TestMethod]
        public void TestSendOneRecipient()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            string testMessageContent = "This is a test";
            int numCalls = 0;
            Action<TestMessage> testAction = (testMessage) =>
            {
                ++numCalls;
                if (testMessage.Text != testMessageContent)
                    throw new InvalidOperationException();
            };

            var messageService = new MessageService(loggerFactory);
            messageService.Register<TestMessage>(testAction);
            var message = new TestMessage(testMessageContent);
            messageService.Send(message);

            Assert.AreEqual(1, numCalls);
        }

        [TestMethod]
        public void TestSendMultipleRecipients()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<MessageService> logger = loggerFactory.CreateLogger<MessageService>();

            string testMessageContent = "This is a test";
            int numCalls1 = 0;
            Action<TestMessage> testAction1 = (testMessage) =>
            {
                ++numCalls1;
                if (testMessage.Text != testMessageContent)
                    throw new InvalidOperationException();
            };
            int numCalls2 = 0;
            Action<TestMessage> testAction2 = (testMessage) =>
            {
                ++numCalls2;
                if (testMessage.Text != testMessageContent)
                    throw new InvalidOperationException();
            };

            var messageService = new MessageService(loggerFactory);
            messageService.Register<TestMessage>(testAction1);
            messageService.Register<TestMessage>(testAction2);
            var message = new TestMessage(testMessageContent);
            messageService.Send(message);

            Assert.AreEqual(1, numCalls1);
            Assert.AreEqual(1, numCalls2);
        }

        [TestMethod]
        public void TestSendMultipleSends()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            string testMessageContent = "This is a test";
            int numCalls = 0;
            Action<TestMessage> testAction = (testMessage) =>
            {
                ++numCalls;
                if (testMessage.Text != testMessageContent)
                    throw new InvalidOperationException();
            };

            var messageService = new MessageService(loggerFactory);
            messageService.Register<TestMessage>(testAction);
            var message = new TestMessage(testMessageContent);
            messageService.Send(message);
            messageService.Send(message);
            messageService.Send(message);

            Assert.AreEqual(3, numCalls);
        }

        [TestMethod]
        public void TestUnregister()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            string testMessageContent = "This is a test";
            int numCalls1 = 0;
            Action<TestMessage> testAction1 = (testMessage) =>
            {
                ++numCalls1;
                if (testMessage.Text != testMessageContent)
                    throw new InvalidOperationException();
            };
            int numCalls2 = 0;
            Action<TestMessage> testAction2 = (testMessage) =>
            {
                ++numCalls2;
                if (testMessage.Text != testMessageContent)
                    throw new InvalidOperationException();
            };

            var messageService = new MessageService(loggerFactory);
            messageService.Register<TestMessage>(testAction1);
            messageService.Register<TestMessage>(testAction2);
            messageService.Unregister<TestMessage>(testAction1);
            var message = new TestMessage(testMessageContent);
            messageService.Send(message);

            Assert.AreEqual(0, numCalls1);
            Assert.AreEqual(1, numCalls2);
        }
    }
}
