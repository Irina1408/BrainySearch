$(document).ready(function () {

    window.confirmItemDeletion = function confirmItemDeletion(e, itemName) {
        return confirm("Are you sure you want to delete the " + itemName + "?");
    };

    window.confirmItemDeletionLinkClick = function confirmItemDeletionLinkClick(e, itemName, itemDeleteFormId) {
        var confirmed = confirm("Are you sure you want to delete the " + itemName + "?");
        if (confirmed) {
            //document.getElementById(itemDeleteFormId).submit();
            return true;
        } else {
            return false;
        }
    };

});