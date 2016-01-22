/**
 * Created by Matt on 16/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .controller('ChatController', chatController);

function chatController(notificationService, $scope) {

    // Visible if user is connected
    $scope.visible = true;
    $scope.messages = notificationService.messages;
    $scope.users = notificationService.users;
    $scope.sounds = notificationService.soundsHistory;
    console.log($scope.sounds);
    $scope.expandOnNew = true;
    $scope.sendMessage = notificationService.sendMessage;

    // Sounds history
    $scope.soundsHistory = notificationService.soundsHistory;

}