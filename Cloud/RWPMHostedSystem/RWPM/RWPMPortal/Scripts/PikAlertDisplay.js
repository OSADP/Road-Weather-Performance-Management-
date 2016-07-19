function GetSitesLayer(map)
{
    var siteLayer = new google.maps.Data();

    siteLayer.addListener('click', function (event) {

        siteLayer.forEach(function (feature) {
            feature.setProperty('IsSelected', false);
        });

        UpdateSiteObservationDetailDisplay(event.feature);

        event.feature.setProperty('IsSelected', true);
    });

    siteLayer.setStyle(function (feature) {
        //Start off with Dark Red to catch any other conditions we may not have coded up.
        var pavmentIcon = '../Content/unknown.png';
        var lineColor = 'black';
        var featureProperty = feature.getProperty('Pavement');


        //orage 242,405,14
        //purple 231,143,255

        if ((feature.getProperty('Pavement') == 'dry') ||
            (feature.getProperty('Pavement') == 'dry or wet')) {
            pavmentIcon = '../Content/sunny.png';
            lineColor = 'yellow';
        }

        else if ((feature.getProperty('Pavement') == 'wet') ||
                (feature.getProperty('Pavement') == 'hydroplaning')) {
            pavmentIcon = '../Content/rainy.png';
            lineColor = 'green';
        }

        else if ((feature.getProperty('Pavement') == 'snow') ||
                    (feature.getProperty('Pavement') == 'slick, snowy')) {
            pavmentIcon = '../Content/snowy.png';
            lineColor = 'blue';
        }

        else if ((feature.getProperty('Pavement') == 'icy') ||
                (feature.getProperty('Pavement') == 'slick, icy') ||
            (feature.getProperty('Pavement') == 'ice possible') ||
            (feature.getProperty('Pavement') == 'slick, ice possible') ||
                (feature.getProperty('Pavement') == 'black ice')) {
            pavmentIcon = '../Content/icyroad.png';
            lineColor = '#B935DE';
        }


        if (feature.getProperty('IsSelected') == true)
        {
            var newIcon = {
                url: pavmentIcon,
                scaledSize: new google.maps.Size(43, 50)
            };

            pavmentIcon = newIcon;
        }

        return {
            title: feature.getProperty('Description'),
            icon: pavmentIcon,
            strokeColor: lineColor,
            strokeWeight: 4
        };
    });

    siteLayer.setMap(map);

    return siteLayer;
}

function UpdateSiteObservationDetailDisplay(feature)
{
    var cont = "<h3>Site Oberservations</h3> ";

    cont = cont.concat("<ul>");
    cont = cont.concat("<li><strong>Description</strong>: ");
    cont = cont.concat(feature.getProperty('Description'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Observation Time UTC</strong>: ");
    cont = cont.concat(feature.getProperty('Observation Time'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Observation Time</strong>: ");
    cont = cont.concat(LocalizeTime(feature.getProperty('Observation Time')));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Alert Code</strong>: ");
    cont = cont.concat(feature.getProperty('AlertCode'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Chemical</strong>: ");
    cont = cont.concat(feature.getProperty('Chemical'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Pavement</strong>: ");
    cont = cont.concat(feature.getProperty('Pavement'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Plow</strong>: ");
    cont = cont.concat(feature.getProperty('Plow'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Precipitation</strong>: ");
    cont = cont.concat(feature.getProperty('Precipitation'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Road Temp</strong>: ");
    cont = cont.concat(feature.getProperty('RoadTemp'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Treatment Alert Code</strong>: ");
    cont = cont.concat(feature.getProperty('TreatmentAlertCode'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Visibility</strong>: ");
    cont = cont.concat(feature.getProperty('Visibility'));
    cont = cont.concat("</li>");
    cont = cont.concat("</ul>");

    document.getElementById('info-box').innerHTML = cont;
}

function GetTreatmentsLayer(map)
{
    var treatmentLayer = new google.maps.Data();

    treatmentLayer.addListener('click', function (event) {
        treatmentLayer.forEach(function (feature) {
            feature.setProperty('IsSelected', false);
        });

        UpdateTreatmentDetailDisplay(event.feature);
        event.feature.setProperty('IsSelected', true);
    });

    treatmentLayer.setStyle(function (feature) {
        //Start off with Dark Red to catch any other conditions we may not have coded up.
        var pavmentIcon = '../Content/unknown.png';
        var lineColor = 'black';
        var featureProperty = feature.getProperty('Plow');


        //orage 242,405,14
        //purple 231,143,255

        if (feature.getProperty('Plow') != 'none') {
            pavmentIcon = '../Content/plowtruck.png';
            lineColor = '#F2690E';
        }
        else if (feature.getProperty('Chemical') != 'none') {
            pavmentIcon = '../Content/salttruck.png';
            lineColor = '#E78FFF';
        }

        if (feature.getProperty('IsSelected') == true) {
            var newIcon = {
                url: pavmentIcon,
                scaledSize: new google.maps.Size(43, 50)
            };

            pavmentIcon = newIcon;
        }


        return {
            title: feature.getProperty('Description'),
            icon: pavmentIcon,
            strokeColor: lineColor,
            strokeWeight: 4
        };
    });

    treatmentLayer.setMap(map);

    return treatmentLayer;
}

function UpdateTreatmentDetailDisplay(feature)
{
    var cont = "<h3>Site Treatment</h3> ";

    cont = cont.concat("<ul>");
    cont = cont.concat("<li><strong>Description</strong>: ");
    cont = cont.concat(feature.getProperty('Description'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Observation Time UTC</strong>: ");
    cont = cont.concat(feature.getProperty('Observation Time'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Observation Time</strong>: ");
    cont = cont.concat(LocalizeTime(feature.getProperty('Observation Time')));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Chemical</strong>: ");
    cont = cont.concat(feature.getProperty('Chemical'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Plow</strong>: ");
    cont = cont.concat(feature.getProperty('Plow'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Road Temp</strong>: ");
    cont = cont.concat(feature.getProperty('RoadTemp'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li><strong>Treatment Alert Code</strong>: ");
    cont = cont.concat(feature.getProperty('TreatmentAlertCode'));
    cont = cont.concat("</li>");
    cont = cont.concat("</ul>");

    document.getElementById('info-box').innerHTML = cont;
}

function ClearSelected()
{
    siteLayer.forEach(function (feature) {
        feature.setProperty('IsSelected', false);
    });

    treatmentLayer.forEach(function (feature) {
        feature.setProperty('IsSelected', false);
    });
    document.getElementById('info-box').innerHTML = "";
}