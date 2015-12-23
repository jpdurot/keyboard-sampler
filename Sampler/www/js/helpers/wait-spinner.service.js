'use strict';

angular
    .module('samplereApp')
    .factory('waitSpinnerService', waitSpinnerService);

/**
 * waitSpinner service: displays waitSpinners to make user wait.
 */
function waitSpinnerService() {

  /*
   * Public interface
   */

  var service = {};

  /**
   * Shows a spinner for long operations
   */
  service.showSpinner = function() {
    displaySpinner = true;
  };

  /**
   * Hides the spinner
   */
  service.hideSpinner = function() {
    displaySpinner = false;
  };

  /**
   * Getter for waitSpinners
   */
  service.isDisplaySpinner = function() {
    return displaySpinner;
  };

  /*
   * Internal
   */

  var displaySpinner = false;

  return service;
}