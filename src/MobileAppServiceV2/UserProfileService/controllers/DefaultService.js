'use strict';

exports.userProfileGET = function(args, res, next) {
  /**
   * List all profiles
   *
   * returns List
   **/
  var examples = {};
  examples['application/json'] = [ {
  "createdAt" : "2000-01-23",
  "firstName" : "aeiou",
  "lastName" : "aeiou",
  "userId" : "aeiou",
  "profilePictureUri" : "",
  "updatedAt" : "2000-01-23"
} ];
  if (Object.keys(examples).length > 0) {
    res.setHeader('Content-Type', 'application/json');
    res.end(JSON.stringify(examples[Object.keys(examples)[0]] || {}, null, 2));
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

