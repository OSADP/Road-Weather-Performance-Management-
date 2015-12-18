
function GetQwarnLayer(map)
{
    var qwarnLayer = new google.maps.Data();

    qwarnLayer.setStyle(function (event) {

        var icon = '../Content/unknown.png';
        if (feature.getProperty('IsSelected') == true) {
            var newIcon = {
                url: icon,
                scaledSize: new google.maps.Size(43, 50)
            };

            icon = newIcon;
        }


        return {
            icon: icon,
            strokeColor: '#9A3243',
            strokeOpacity: '1.0',
            strokeWeight: '4.0'
        };
    });

    qwarnLayer.addListener('click', function (event) {
        qwarnLayer.forEach(function (feature) {
            feature.setProperty('IsSelected', false);
        });
        UpdateQWarnDisplay(event.feature);
        event.feature.setProperty('IsSelected', true);
    });

    qwarnLayer.addListener('mouseout', function (event) {
        
    });
    qwarnLayer.setMap(map);

    return qwarnLayer;
}

function ClearInfloDisplay()
{
    qwarnLayer.forEach(function (feature) {
        feature.setProperty('IsSelected', false);
    });

    speedHarmLayer.forEach(function (feature) {
        feature.setProperty('IsSelected', false);
    });

    
    
    
    document.getElementById('info-box').innerHTML = "";
    $("#qWarnBtnDiv").hide();
    $("#spdHarmBtnDiv").hide();

}

function ClearVehiclesDisplay()
{
    document.getElementById('info-box').innerHTML = "";
    vehiclesLayer.forEach(function (feature) {
        feature.setProperty('IsSelected', false);
    });

}
function ClearSpeedSensorDisplay()
{
    document.getElementById('info-box').innerHTML = "";
    speedSensorLayer.forEach(function (feature) {
        feature.setProperty('IsSelected', false);
    });
}
function UpdateQWarnDisplay(feature)
{
    var cont = "<h3>QWarn Properties</h3> ";
    cont = cont.concat("<ul>");
    cont = cont.concat("<li>Distance: ")
    cont = cont.concat(feature.getProperty('Distance'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Front Of Queue: ")
    cont = cont.concat(feature.getProperty('FOQMMLocation'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Back Of Queue: ")
    cont = cont.concat(feature.getProperty('BOQMMLocation'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Date UTC: ")
    cont = cont.concat(feature.getProperty('DateGenerated'));
    cont = cont.concat("<li>Date: ")
    cont = cont.concat(LocalizeTime(feature.getProperty('DateGenerated')));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Valid Duration: ")
    cont = cont.concat(feature.getProperty('ValidityDuration'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Rate of Growth: ")
    cont = cont.concat(feature.getProperty('RateOfQueueGrowth'));
    cont = cont.concat("</li>");
    cont = cont.concat("</ul>");

    document.getElementById('info-box').innerHTML = cont;

    $("#selectedRoadwayId").val(event.feature.getProperty('RoadwayId'));
    $("#selectedEndMM").val(event.feature.getProperty('FOQMMLocation'));
    selectedEvent = event.feature;
    document.getElementById('rmvAlertErr').innerHTML = '';
    //show qwarn button's div, hide spd harm's div.
    $("#spdHarmBtnDiv").hide();
    $("#qWarnBtnDiv").show();
}

function UpdateSpeedHarmDisplay(feature)
{
    var cont = "<h3>SpeedHarm Properties</h3> ";
    cont = cont.concat("<ul>");
    cont = cont.concat("<li>RecommendedSpeed: ")
    cont = cont.concat(feature.getProperty('RecommendedSpeed'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>BeginMM: ")
    cont = cont.concat(feature.getProperty('BeginMM'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Justification: ")
    cont = cont.concat(feature.getProperty('Justification'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Distance: ")
    cont = cont.concat(feature.getProperty('Distance'));
    cont = cont.concat("</li>");
    cont = cont.concat("</ul>");

    document.getElementById('info-box').innerHTML = cont;

    $("#selectedRoadwayId").val(feature.getProperty('RoadwayId'));
    $("#selectedBeginMM").val(feature.getProperty('BeginMM'));
    $("#selectedEndMM").val(feature.getProperty('EndMM'));
    selectedEvent = feature;
    document.getElementById('rmvAlertErr').innerHTML = '';
    //show spd harm's button's div, hide qwarns's button's div.
    $("#spdHarmBtnDiv").show();
    $("#qWarnBtnDiv").hide();
}

function GetSpeedHarmLayer(map)
{
    var speedHarmLayer = new google.maps.Data();

    speedHarmLayer.setStyle(function (feature) {
        var icon = '../Content/dkpurple.png';
        if (feature.getProperty('IsSelected') == true) {
            var newIcon = {
                url: icon,
                scaledSize: new google.maps.Size(43, 50)
            };

            icon = newIcon;
        }


        return {
            icon: icon,
            strokeColor: '#9A3243',
            strokeOpacity: '1.0',
            strokeWeight: '4.0'
        };
    });

    speedHarmLayer.addListener('click', function (event) {
        speedHarmLayer.forEach(function (feature) {
            feature.setProperty('IsSelected', false);
        });
        UpdateSpeedHarmDisplay(event.feature)
        event.feature.setProperty('IsSelected', true);
    });

    speedHarmLayer.setMap(map);

    return speedHarmLayer;
}

function GetVehiclesLayer(map) {
    var vehiclesLayer = new google.maps.Data();

    vehiclesLayer.setStyle(function (feature) {
        var icon = '../Content/car.png';
        if (feature.getProperty('IsSelected') == true) {
            var newIcon = {
                url: icon,
                scaledSize: new google.maps.Size(43, 50)
            };

            icon = newIcon;
        }


        return {
            icon: icon
        };
    });

    vehiclesLayer.addListener('click', function (event) {
        vehiclesLayer.forEach(function (feature) {
            feature.setProperty('IsSelected', false);
        });
        UpdateVehiclesDisplay(event.feature)
        event.feature.setProperty('IsSelected', true);
    });

    vehiclesLayer.setMap(map);

    return vehiclesLayer;
}

function UpdateVehiclesDisplay(feature)
{
    var cont = "<h3>Vehicle Properties</h3> ";
    cont = cont.concat("<ul>");
    cont = cont.concat("<li>GpsHeading: ")
    cont = cont.concat(feature.getProperty('GpsHeading'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>GpsSpeed: ")
    cont = cont.concat(feature.getProperty('GpsSpeed'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Speed: ")
    cont = cont.concat(feature.getProperty('Speed'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>HeadlightStatus: ")
    cont = cont.concat(feature.getProperty('HeadlightStatus'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>AirTemperature: ")
    cont = cont.concat(feature.getProperty('AirTemperature'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>AtmosphericPressure: ")
    cont = cont.concat(feature.getProperty('AtmosphericPressure'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>LeftFrontWheelSpeed: ")
    cont = cont.concat(feature.getProperty('LeftFrontWheelSpeed'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>LeftRearWheelSpeed: ")
    cont = cont.concat(feature.getProperty('LeftRearWheelSpeed'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>RightFrontWheelSpeed: ")
    cont = cont.concat(feature.getProperty('RightFrontWheelSpeed'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>RightRearWheelSpeed: ")
    cont = cont.concat(feature.getProperty('RightRearWheelSpeed'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>SteeringWheelAngle: ")
    cont = cont.concat(feature.getProperty('SteeringWheelAngle'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>WiperStatus: ")
    cont = cont.concat(feature.getProperty('WiperStatus'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>DateGenerated UTC: ")
    cont = cont.concat(feature.getProperty('DateGenerated'));
    cont = cont.concat("<li>DateGenerated: ")
    cont = cont.concat(LocalizeTime(feature.getProperty('DateGenerated')));
    cont = cont.concat("</li>");
    cont = cont.concat("</ul>");

    document.getElementById('info-box').innerHTML = cont;
}

function UpdateSpeedSensorDisplay(feature)
{
    var cont = "<h3>Speed Sensor Properties</h3> ";
    cont = cont.concat("<ul>");
    cont = cont.concat("<li>BeginTime UTC: ")
    cont = cont.concat(feature.getProperty('BeginTime'));
    cont = cont.concat("<li>BeginTime: ")
    cont = cont.concat(LocalizeTime(feature.getProperty('BeginTime')));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Volume: ")
    cont = cont.concat(feature.getProperty('Volume'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>AvgSpeed: ")
    cont = cont.concat(feature.getProperty('AvgSpeed'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Congested: ")
    cont = cont.concat(feature.getProperty('Congested'));
    cont = cont.concat("</li>");
    cont = cont.concat("<li>Occupancy: ")
    cont = cont.concat(feature.getProperty('Occupancy'));
    cont = cont.concat("</li>");
    cont = cont.concat("</ul>");

    document.getElementById('info-box').innerHTML = cont;
}

function GetSpeedSensorsLayer(map)
{
    speedSensorLayer = new google.maps.Data();

    speedSensorLayer.setStyle(function (feature) {
        var speedIcon = '../Content/dkred.png';
        var speedNumber = Number(feature.getProperty('AvgSpeed'));

        if (speedNumber >= 65) {
            speedIcon = '../Content/speed65.png';
        }
        else if (speedNumber >= 60) {
            speedIcon = '../Content/speed60.png';
        }
        else if (speedNumber >= 55) {
            speedIcon = '../Content/speed55.png';
        }
        else if (speedNumber >= 50) {
            speedIcon = '../Content/speed50.png';
        }
        else if (speedNumber >= 45) {
            speedIcon = '../Content/speed45.png';
        }
        else if (speedNumber >= 40) {
            speedIcon = '../Content/speed40.png';
        }
        else if (speedNumber >= 35) {
            speedIcon = '../Content/speed35.png';
        }
        else if (speedNumber >= 30) {
            speedIcon = '../Content/speed30.png';
        }
        else if (speedNumber >= 25) {
            speedIcon = '../Content/speed25.png';
        }
        else if (speedNumber >= 20) {
            speedIcon = '../Content/speed20.png';
        }
        else if (speedNumber >= 15) {
            speedIcon = '../Content/speed15.png';
        }
        else if (speedNumber >= 10) {
            speedIcon = '../Content/speed10.png';
        }
        else {
            speedIcon = '../Content/speed5.png';
        }

        if (feature.getProperty('IsSelected') == true) {
            var newIcon = {
                url: speedIcon,
                scaledSize: new google.maps.Size(43, 50)
            };

            speedIcon = newIcon;
        }

        return {
            icon: speedIcon
        }
    });

    speedSensorLayer.addListener('click', function (event) {
        speedSensorLayer.forEach(function (feature) {
            feature.setProperty('IsSelected', false);
        });
        UpdateSpeedSensorDisplay(event.feature);
        event.feature.setProperty('IsSelected', true);
    });

    speedSensorLayer.setMap(map);

    return speedSensorLayer;
}

function SetIsSelected(data, feature)
{
    data.features.forEach(function (newFeature) {
        if (newFeature.properties['Description'] == feature.getProperty('Description')) {
            if (feature.getProperty('IsSelected') == true) {
                newFeature.properties['IsSelected'] = true;
            }
        }
    });
}

function SetIsSelectedVehicle(data, feature) {
    data.features.forEach(function (newFeature) {
        if (newFeature.properties['NomadicDeviceId'] == feature.getProperty('NomadicDeviceId')) {
            if (feature.getProperty('IsSelected') == true) {
                newFeature.properties['IsSelected'] = true;
            }
        }
    });
}

function SetIsSelectedSpeedSensor(data, feature) {
    data.features.forEach(function (newFeature) {
        if (newFeature.properties['DZId'] == feature.getProperty('DZId')) {
            if (feature.getProperty('IsSelected') == true) {
                newFeature.properties['IsSelected'] = true;
            }
        }
    });
}

function LocalizeTime(date)
{
    var date2 = new Date(date + "Z");

    var s1 = date2.toLocaleString();

    return s1;
}