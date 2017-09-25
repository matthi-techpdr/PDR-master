function Map(selector) {
    window.map =
        new GMaps({
                div: selector,
                lat: 39.50,
                lng: -98.35,
                zoom: 4
            });
}

Map.RouteColors =
    [
    "ff0000",
    "00ff4e",
    "3c00ff",
    "00e4ff",
    "ea00ff",
    "00ff4e",
    "a89f54",
    "10471c",
    "101847",
    "9064cb"
];

Map.prototype.RenderLastLocations = function (models) {
    _.each(models, function (model, i) {
        var location = model.get('Location');
        var lat = location.LatLng.lat;
        var lng = location.LatLng.lng;
        var title = model.get('Name') + " : " + location.Date;
        var marker = new google.maps.Marker({ position: new google.maps.LatLng(lat, lng), title: title});
        window.map.addMarker(marker);
    });
};

Map.prototype.RenderRoutes = function (models, highlightRouteLicense) {
    Map.Routes = [];

    window.coordinates = [];
    var cluster = new DCluster(window.map);
    for (var i = 0; i < models.length; i++) {
        var routePath = [];
        var model = models[i];
        var clusterCoordinates = [];
        for (var j = 0; j < model.Locations.length; j++) {
            var locationInfo = model.Locations[j];
            var latLng = [locationInfo.LatLng.lat, locationInfo.LatLng.lng];
            var clusterModel = { coordinates: new google.maps.LatLng(locationInfo.LatLng.lat, locationInfo.LatLng.lng), comment: model.Name + ": " + locationInfo.Date };
            clusterCoordinates.push(clusterModel);
            routePath.push(latLng);
        }

        window.coordinates.push(clusterCoordinates);

        window.map.drawPolyline({
            path: routePath,
            strokeColor: '#' + Map.RouteColors[i],
            strokeOpacity: model.LicenseId == highlightRouteLicense ? 1 : 0.5,
            strokeWeight: model.LicenseId == highlightRouteLicense ? 5 : 3,
            click:
                    function () {
                        var lineColor = this.strokeColor[0] == '#' ? this.strokeColor : '#' + this.strokeColor;
                        $('#legend').find('tr').each(function () {
                            var trColor = $(this).find('div').getHexColor();
                            if (trColor == lineColor.toLowerCase()) {
                                $(this).click();
                            }
                        });
                    }
        });

        var drawMarkers = function () {
            window.map.removeMarkers();
            for (var k = 0; k < window.coordinates.length; k++) {
                var color = Map.RouteColors[k];
                var locations = function () { return window.coordinates[k]; };
                cluster.SetMarkers(locations(), color);
            }
        };

        drawMarkers();

        google.maps.event.addListener(window.map.map, 'zoom_changed', function () {
            drawMarkers();
        });
    };
};

Map.GetUserColors = function (models) {
    var userColors = [];
    _.each(models, function (model, i) {
        var name = model.get('Name');
        var license = model.get('LicenseId');
        var color = Map.RouteColors[i];
        userColors.push({ user: i + 1 + ". " + name, color: '#' + color, license: license });
    });

    return userColors;
};













































































