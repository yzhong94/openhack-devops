'use strict';

var url = require('url');

var Default = require('./DefaultService');

module.exports.userProfileGET = function userProfileGET (req, res, next) {
  Default.userProfileGET(req.swagger.params, res, next);
};

module.exports.userProfilePOST = function userProfilePOST (req, res, next) {
  Default.userProfilePOST(req.swagger.params, res, next);
};
