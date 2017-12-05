// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Defines *all* the logger messages produced by static files
    /// </summary>
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception> _logMethodNotSupported;
        private static readonly Action<ILogger, string, string, string, Exception> _logFileServed;
        private static readonly Action<ILogger, string, Exception> _logPathMismatch;
        private static readonly Action<ILogger, string, Exception> _logFileTypeNotSupported;
        private static readonly Action<ILogger, string, Exception> _logFileNotFound;
        private static readonly Action<ILogger, string, Exception> _logPathNotModified;
        private static readonly Action<ILogger, string, Exception> _logPreconditionFailed;
        private static readonly Action<ILogger, int, string, Exception> _logHandled;
        private static readonly Action<ILogger, string, Exception> _logRangeNotSatisfiable;
        private static readonly Action<ILogger, StringValues, string, string, Exception> _logSendingFileRange;
        private static readonly Action<ILogger, StringValues, string, string, Exception> _logCopyingFileRange;
        private static readonly Action<ILogger, long, string, string, string, Exception> _logCopyingBytesToResponse;
        private static readonly Action<ILogger, Exception> _logWriteCancelled;
        private static readonly Action<ILogger, string, string, string, Exception> _logNotAuthenticated;
        private static readonly Action<ILogger, string, string, string, Exception> _logNotAuthorized;
        private static readonly Action<ILogger, string, Exception> _logTransportMethod;

        static LoggerExtensions()
        {
            _logMethodNotSupported = LoggerMessage.Define<string>(
                                                                  logLevel: LogLevel.Trace,
                                                                  eventId: 1,
                                                                  formatString: "{Method} requests are not supported");
            _logFileServed = LoggerMessage.Define<string, string, string>(
                                                                  logLevel: LogLevel.Information,
                                                                  eventId: 2,
                                                                  formatString: "Sending file to {User}. Request path: '{VirtualPath}'. Physical path: '{PhysicalPath}'");
            _logPathMismatch = LoggerMessage.Define<string>(
                                                            logLevel: LogLevel.Trace,
                                                            eventId: 3,
                                                            formatString: "The request path {Path} does not match the path filter");
            _logFileTypeNotSupported = LoggerMessage.Define<string>(
                                                                    logLevel: LogLevel.Trace,
                                                                    eventId: 4,
                                                                    formatString: "The request path {Path} does not match a supported file type");
            _logFileNotFound = LoggerMessage.Define<string>(
                                                            logLevel: LogLevel.Trace,
                                                            eventId: 5,
                                                            formatString: "The request path {Path} does not match an existing file");
            _logPathNotModified = LoggerMessage.Define<string>(
                                                               logLevel: LogLevel.Trace,
                                                               eventId: 6,
                                                               formatString: "The file {Path} was not modified");
            _logPreconditionFailed = LoggerMessage.Define<string>(
                                                                  logLevel: LogLevel.Trace,
                                                                  eventId: 7,
                                                                  formatString: "Precondition for {Path} failed");
            _logHandled = LoggerMessage.Define<int, string>(
                                                            logLevel: LogLevel.Debug,
                                                            eventId: 8,
                                                            formatString: "Handled. Status code: {StatusCode} File: {Path}");
            _logRangeNotSatisfiable = LoggerMessage.Define<string>(
                                                                   logLevel: LogLevel.Warning,
                                                                   eventId: 9,
                                                                   formatString: "Range not satisfiable for {Path}");
            _logSendingFileRange = LoggerMessage.Define<StringValues, string, string>(
                                                                              logLevel: LogLevel.Debug,
                                                                              eventId: 10,
                                                                              formatString: "Sending {Range} of file {Path} to {User}");
            _logCopyingFileRange = LoggerMessage.Define<StringValues, string, string>(
                                                                              logLevel: LogLevel.Debug,
                                                                              eventId: 11,
                                                                              formatString: "Copying {Range} of file {Path} to the response body for {User}");
            _logCopyingBytesToResponse = LoggerMessage.Define<long, string, string, string>(
                                                                                    logLevel: LogLevel.Debug,
                                                                                    eventId: 12,
                                                                                    formatString: "Copying bytes {Start}-{End} of file {Path} to response body for {User}");
            _logWriteCancelled = LoggerMessage.Define(
                                                      logLevel: LogLevel.Debug,
                                                      eventId: 14,
                                                      formatString: "The file transmission was cancelled");
            _logNotAuthenticated = LoggerMessage.Define<string, string, string>(
                                                                logLevel: LogLevel.Information,
                                                                eventId: 15,
                                                                formatString:
                                                                "Request from {ip} not authenticated to library {Library} for file {Path}");
            _logNotAuthorized = LoggerMessage.Define<string, string, string>(
                                                             logLevel: LogLevel.Information,
                                                             eventId: 16,
                                                             formatString: "Request for {User} not authorized to library {Library} for file {Path}");
            _logTransportMethod = LoggerMessage.Define<string>(
                                                               logLevel: LogLevel.Trace,
                                                               eventId: 17,
                                                               formatString: "Using {Method} transport method.");
        }

        public static void LogRequestMethodNotSupported(this ILogger logger, string method)
        {
            _logMethodNotSupported(logger, method, null);
        }

        public static void LogFileServed(this ILogger logger, string virtualPath, string physicalPath, string user)
        {
            if (string.IsNullOrEmpty(physicalPath))
            {
                physicalPath = "N/A";
            }
            if (string.IsNullOrEmpty(user))
            {
                user = "Anonymous";
            }
            _logFileServed(logger, virtualPath, physicalPath, user, null);
        }

        public static void LogPathMismatch(this ILogger logger, string path)
        {
            _logPathMismatch(logger, path, null);
        }

        public static void LogFileTypeNotSupported(this ILogger logger, string path)
        {
            _logFileTypeNotSupported(logger, path, null);
        }

        public static void LogFileNotFound(this ILogger logger, string path)
        {
            _logFileNotFound(logger, path, null);
        }

        public static void LogPathNotModified(this ILogger logger, string path)
        {
            _logPathNotModified(logger, path, null);
        }

        public static void LogPreconditionFailed(this ILogger logger, string path)
        {
            _logPreconditionFailed(logger, path, null);
        }

        public static void LogHandled(this ILogger logger, int statusCode, string path)
        {
            _logHandled(logger, statusCode, path, null);
        }

        public static void LogRangeNotSatisfiable(this ILogger logger, string path)
        {
            _logRangeNotSatisfiable(logger, path, null);
        }

        public static void LogSendingFileRange(this ILogger logger, StringValues range, string path, string user)
        {
            _logSendingFileRange(logger, range, path, user, null);
        }

        public static void LogCopyingFileRange(this ILogger logger, StringValues range, string path, string user)
        {
            _logCopyingFileRange(logger, range, path, user, null);
        }

        public static void LogCopyingBytesToResponse(this ILogger logger, long start, long? end, string path, string user)
        {
            _logCopyingBytesToResponse(
                                       logger,
                                       start,
                                       end != null ? end.ToString() : "*",
                                       path,
                                       user,
                                       null);
        }

        public static void LogWriteCancelled(this ILogger logger, Exception ex)
        {
            _logWriteCancelled(logger, ex);
        }

        public static void LogNotAuthenticated(this ILogger logger, string library, string path, string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                ip = "N/A";
            }
            _logNotAuthenticated(logger, library, path, ip, null);
        }

        public static void LogNotAuthorized(this ILogger logger, string library, string path, string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                user = "Anonymous";
            }
            _logNotAuthorized(logger, library, path, user, null);
        }

        public static void LogTransportMethod(this ILogger logger, string method)
        {
            _logTransportMethod(logger, method, null);
        }
    }
}