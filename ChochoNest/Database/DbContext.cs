using System;
using DotNetEnv;

namespace ChochoNest.Database
{
    public class DbContext
    {
        private string? _dbHost;
        private string? _dbPort;
        private string? _dbUser;
        private string? _dbPassword;
        private string? _dbName;
        private string? _sslMode;
        private string? _channelBinding;

        public string connStr;

        public DbContext()
        {
            Env.Load();

            _dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            _dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            _dbUser = Environment.GetEnvironmentVariable("DB_USER");
            _dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            _dbName = Environment.GetEnvironmentVariable("DB_NAME");
            _sslMode = Environment.GetEnvironmentVariable("SSL_Mode") ?? Environment.GetEnvironmentVariable("SSL_MODE");
            _channelBinding = Environment.GetEnvironmentVariable("Channel_Binding") ?? Environment.GetEnvironmentVariable("CHANNEL_BINDING");


            connStr = $"Host={_dbHost!};Port={_dbPort!};Username={_dbUser!};Password={_dbPassword!};Database={_dbName!};SSL Mode={_sslMode!};Channel Binding={_channelBinding!};Pooling=true;";
        }
    }
}