'use strict';

//const sql = require('mssql')
var Connection = require('tedious').Connection;
var Request = require('tedious').Request;
var config = require('../config/config');
var getresult;

exports.userProfileGET = function(args, res, next) {

  // https://docs.microsoft.com/en-us/azure/sql-database/sql-database-connect-query-nodejs
  //https://github.com/tediousjs/tedious/blob/master/examples/minimal.js

  var connection = new Connection(config);

  // Attempt to connect and execute queries if connection goes through
  connection.on('connect', function(err) 
     {
       if (err) 
         {
            console.log(err)
         }
      else
         {
             queryDatabase()
         }
     }
   );
  
  function queryDatabase()
     { console.log('Reading rows from the Table...');

       request = new Request(
            "select * from UserProfiles"
              );
      connection.execSql(request);
      // Need to get the result back to a meaningful object 
      // console.log(request)
      // res.end(JSON.stringify(request))
     }

    // http://www.dotnetcurry.com/nodejs/1238/connect-sql-server-nodejs-mssql-package - Step 5.  Doesn't work with initial connection, guessing it is because it used the 3.x mssql package (now at 4.x) 

    //  function getUsers() {
    //   var dbConn = new sql.Connection(config);
    //   dbConn.connect().then(function () {
    //       var request = new sql.Request(dbConn);
    //       request.query("select * from UserProfiles").then(function (recordSet) {
    //           console.log(recordSet);
    //           dbConn.close();
    //       }).catch(function (err) {
    //           console.log(err);
    //           dbConn.close();
    //       });
    //   }).catch(function (err) {
    //       console.log(err);
    //   });
    // }

  //https://www.npmjs.com/package/mssql#connection-pools
  // Doesn't actually execute the sql.close() so 2nd request fails.

  // sql.connect(config).then(pool => {
  //   // Query
  //   return pool.request()
  //   //.input('input_parameter', sql.Int, value)
  //   .query('select * from UserProfiles') 
  //   //where Id = ')
  //   }).then(result => {
  //       console.log(req)
  //       res.end(JSON.stringify(result)).then(sql.close())
  //     }).catch(err => {
  //     // ... error checks
  //     })

}

exports.userProfilePOST = function(args, res, next) {
  /**
   * Declares and creates a new profile
   *
   * _profile Profile Details of the profile
   * returns Profile
   **/
  var examples = {};
  examples['application/json'] = {
  "createdAt" : "2000-01-23",
  "firstName" : "aeiou",
  "lastName" : "aeiou",
  "userId" : "aeiou",
  "profilePictureUri" : "",
  "updatedAt" : "2000-01-23"
};
  if (Object.keys(examples).length > 0) {
    res.setHeader('Content-Type', 'application/json');
    res.end(JSON.stringify(examples[Object.keys(examples)[0]] || {}, null, 2));
  } else {
    res.end();
  }
}

