//-----------------------------Models----------------------------------

var LastLocations = Backbone.Collection.extend({
    initialize: function () {
        this.on("reset", function () {
            var view = new LastLocationsView({collection:this});
            var element = view.render().el;
            $('#location').html(element);
            
            var map = new Map('#map');
            map.RenderLastLocations(this.models);
            $('#timeFilter').hide();
        });
    },
    url: 'locations/getlastlocations'
});

var Routes = Backbone.Collection.extend({
    initialize: function () {
        this.on("reset", function () {
            window.collection = this;
            var view = new RoutesView({ collection: this });
            var element = view.render().el;
            $('#location').html(element);
            $('#timeFilter').show();

            var model = this.toJSON();
            var map = new Map('#map');
            map.RenderRoutes(model);

            $('.user').click(function () {
                window.map.removePolylines();
                $('.user').css('color', '#555');
                var license = $(this).attr('license');
                map.RenderRoutes(model, license);
                $(this).css('color', 'blue');
                var currentModel = _.filter(window.collection.toJSON(), function (m) { return m.LicenseId == license; })[0];
                var location = currentModel.Locations[0].LatLng;
                window.map.setCenter(location.lat, location.lng);
            });
        });
    },
    from: function () {
        return $('#from').val() || "";
    },
    to: function () {
        return $('#to').val() || "";
    },
    licenses: function () {
        return $("#gpsReportsGrid").jqGrid('getGridParam', 'selarrrow').join(",");
    },
    url: function () {
        return 'locations/getroutes?' +
            'from=' + this.from() +
            "&to=" + this.to() +
            "&licensesId=" + this.licenses();
    }
});


//------------------------Views-------------------------------------------

var LastLocationsView = Backbone.View.extend({
    initialize: function () {
        Helpers.RemoveActiveButtons();
        _.bindAll(this);
    },
    render: function () {
        var template = $('#last-locations-template').tmpl();
        $(this.el).html(template);
        return this;
    }
});

var RoutesView = Backbone.View.extend({
    initialize: function () {
        Helpers.RemoveActiveButtons();
    },

    render: function () {
        var template = $('#routes-template').tmpl();
        $(this.el).html(template);
        var models = this.collection.models;
        var rows = this.collection.models.length != 0
            ? $('#legend-template').tmpl(Map.GetUserColors(models))
            : '<tr><td>No routes</td></tr>';
        $(template).find('table').append(rows);
        return this;
    }
});

