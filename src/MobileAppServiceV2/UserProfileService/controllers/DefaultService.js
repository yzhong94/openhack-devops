//'use strict';

//const sql = require('mssql')
var Connection = require('tedious').Connection;
var Request = require('tedious').Request;
var config = require('../config/config');

// https://docs.microsoft.com/en-us/azure/sql-database/sql-database-connect-query-nodejs
// https://github.com/tediousjs/tedious/blob/master/examples/minimal.js
var connection = new Connection(config);

connection.on('connect', function(err) 
   {
     if (err) 
       {
          console.log(err)
       }
    else
       {
          console.log("connection working")
       }
   }
 );

function queryDatabase(sqlquery){ 
  console.log('Reading rows from the Table...');

  // Read all rows from table
  request = new Request(
    sqlquery,
      function(err, rowCount, rows) {
        console.log(rowCount + ' row(s) returned');
      }
  );

  request.on('row', function(columns) {
    columns.forEach(function(column) {
      console.log("%s\t%s", column.metadata.colName, column.value);
      var tempobj = column.metadata.value;
      return tempobj;
    });
  });
  connection.execSql(request);
}

exports.userProfileGET = function(args, res, next) {

  var queryString = "SELECT * FROM UserProfiles";
  var examples = queryDatabase(queryString);
  if (Object.keys(examples).length > 0) {
    res.setHeader('Content-Type', 'application/json');
    res.end(JSON.stringify(examples));
  } else {
    res.end();
  }

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

