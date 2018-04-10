const sql = require('mssql')
 
sql.connect(config).then(pool => {
    // Query
    
    return pool.request()
    .input('input_parameter', sql.Int, value)
    .query('select * from mytable where id = @input_parameter')
}).then(result => {
    console.dir(result)
    
    // Stored procedure
    
    return pool.request()
    .input('input_parameter', sql.Int, value)
    .output('output_parameter', sql.VarChar(50))
    .execute('procedure_name')
}).then(result => {
    console.dir(result)
}).catch(err => {
    // ... error checks
})
 
sql.on('error', err => {
    // ... error handler
})