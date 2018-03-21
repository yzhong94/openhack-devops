const config = {
    user: process.env.SQL_USER || '',
    password: process.env.SQL_PASSWORD || '',
    server: process.env.SQL_SERVER || '', // You can use 'localhost\\instance' to connect to named instance
    database: process.env.SQL_DBNAME || '',
 
    options: {
        encrypt: true, // Use this if you're on Windows Azure
        MultipleActiveResultSets: false,
        TrustServerCertificate: false
        // Persist Security Info=False;Connection Timeout=30
    }
}

exports = module.exports = config;
