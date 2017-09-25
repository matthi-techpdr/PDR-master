function DCluster(gmap,options) {
    this.gmap = gmap;	
	options = options || {};

    var getMarkerImage = function(color, size, label) {
        if (options.imageUrl == undefined) {
            return "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=" + label + "|" + color + "|000000";
        }

        return options.imageUrl + "?color=" + color + "&size=" + size + "&label=" + label;
    };
	
    this.locations = {};
    var radiuses = function () {
        var radiuses = {};
        radiuses[0] = 1000000;
        radiuses[1] = 800000;
        radiuses[2] = 500000;
        radiuses[3] = 200000;
        radiuses[4] = 200000;
        radiuses[5] = 100000;
        radiuses[6] = 50000;
        radiuses[7] = 20000;
        radiuses[8] = 20000;
        radiuses[9] = 10000;
        radiuses[10] = 5000;
        radiuses[11] = 2000;
        radiuses[12] = 1000;
        radiuses[13] = 500;
        radiuses[14] = 200;
        radiuses[15] = 100;
        radiuses[16] = 50;
        radiuses[17] = 20;
        radiuses[18] = 20;
        radiuses[19] = 10;
        radiuses[20] = 5;
        radiuses[21] = 1;
        return radiuses;
    }.call();



    var getComment = function(markers) {
        var comments = '<div class="markerinfo">';
        for (var i = 0; i < markers.length; i++) {
            comments += '<p style="padding-top:3px;">' + markers[i].comment + '</p>' ;
        }
        return comments + '</div>';
    };

var createArea = function (location) {
    var areaMarkers = [];
    var radius = radiuses[this.gmap.map.getZoom()];
    for (var i = 0; i < this.locations.length; i++) {
        var l = this.locations[i];
        var distance = google.maps.geometry.spherical.computeDistanceBetween(location.coordinates, l.coordinates);        
        if (distance < radius) {
            areaMarkers.push(l);
            this.locations.splice(i, 1);
            i = -1;
        }
    }

    return areaMarkers;
};

var getAllAreas = function () {
    var areas = [];
    while (this.locations.length != 0) {
        var location = this.locations[0];
        var areaMarkers = createArea.apply(this,[location]);
        areas.push(areaMarkers);
    }

    return areas;
};

DCluster.prototype.SetMarkers = function (locations, color) {
    this.locations = locations.slice();
    var markersAreas = getAllAreas.apply(this);
    for (var i = 0; i < markersAreas.length; i++) {
        var marker = markersAreas[i];
        var icon = getMarkerImage(color, null, marker.length);

        var pin = new google.maps.MarkerImage(icon, null, null, null, new google.maps.Size(30, 32));

        var lat = marker[0].coordinates.lat();
        var lng = marker[0].coordinates.lng();

        this.gmap.addMarker({
            lat: lat,
            lng: lng,
            icon: icon,
            infoWindow: {
                content: getComment(marker),
                maxWidth: 500
            }
        });
    }
};	
}



