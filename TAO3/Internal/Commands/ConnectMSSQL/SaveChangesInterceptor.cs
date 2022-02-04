using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TAO3.Internal.Commands.ConnectMSSQL
{
    public class SaveChangesInterceptor : DbCommandInterceptor, IDbCommandInterceptor
    {
        private readonly DbCommandFormatter _formatter;
        private readonly List<string> _queries;

        public List<string> SqlQueries => _queries;

        public SaveChangesInterceptor()
        {
            _formatter = new DbCommandFormatter();
            _queries = new List<string>();
        }

        public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
        {
            AppendCommand(command);
            return base.NonQueryExecuted(command, eventData, result);
        }

        public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            AppendCommand(command);
            return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            AppendCommand(command);
            return base.ReaderExecuted(command, eventData, result);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            AppendCommand(command);
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
        {
            AppendCommand(command);
            return base.ScalarExecuted(command, eventData, result);
        }

        public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
        {
            AppendCommand(command);
            return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
        }

        private void AppendCommand(DbCommand command)
        {
            _queries.Add(_formatter.Format(command));
        }
    }
}
