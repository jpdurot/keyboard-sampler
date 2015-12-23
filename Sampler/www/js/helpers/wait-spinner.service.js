'use strict';

angular
    .module('samplereApp')
    .factory('waitSpinnerService', waitSpinnerService);

/**
 * waitSpinner service: displays waitSpinners to make user wait.
 */
function waitSpinnerService($rootScope) {

  /*
   * Public interface
   */

  var service = {};

  /**
   * Shows a spinner for long operations
   */
  service.showSpinner = function() {
    service.displaySpinner = true;
    $rootScope.$broadcast('spinner');
  };

  /**
   * Hides the spinner
   */
  service.hideSpinner = function() {
    service.displaySpinner = false;
    $rootScope.$broadcast('spinner');
  };

  /*
   * Internal
   */

  service.displaySpinner = false;

  return service;
}