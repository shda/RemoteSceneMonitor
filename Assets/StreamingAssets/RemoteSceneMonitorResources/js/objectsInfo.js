var timerId = null;
var idGameObject;

var positionUi;
var rotationUi;
var scaleUi;

var activeSelf;

var delayUpdate = 500;

function ShowDataToGameObjectById(id){
  idGameObject = id;
  UpdateDelayInput();
  if(timerId == null){
      ConnectTransformUi();
      ConnectEnableButtonUi();
      timerId = setTimeout(function tick() {
        RequestDataToUi(function(){
          timerId = setTimeout(tick, delayUpdate);
        });
      })
  }
  else{
    RequestDataToUi();
  }
}

function UpdateDelayInput(){
  $("#deleyUpdate").change(function() {
     var value = parseInt($("#deleyUpdate").val());
     if(Number.isInteger(value)){
       if(value < 100){
         value = 100;
       }
       delayUpdate = value;
       $("#deleyUpdate").val(delayUpdate);
     }
  });
}

function RequestDataToUi(callback){
  utils.getJSON("/gameObjectInfo?" + CreateParams({id : idGameObject}) , function(err , data){
    if(err === null && data != null){
      SetValuesComponents(data);
    }
    else{
      SetValuesComponents(null);
    }
    if(callback != null){
      callback();
    }

  }, 500);
}

function GetActiveSelf(){
  return activeSelf[0].checked;
}

function SetActiveSelf(value){
  activeSelf[0].checked = value;
}

function ConnectEnableButtonUi(){
  activeSelf = $('#activeSelf');
  activeSelf.change(function(){
    OnChangeEnableGameobject();
  });
}


function ConnectTransformUi(){
  function ConnectUi(namesArray , onChangeValue){
    var vector3dUi = {
      x: $(namesArray.x),
      y: $(namesArray.y),
      z: $(namesArray.z),
      setX : function(val){
        if(this.x[0] != document.activeElement){
          this.x.val(val);
        }
      },
      setY: function(val){
        if(y[0] != document.activeElement){
          y.val(val);
        }
      },
      setZ : function(val){
        if(y[0] != document.activeElement){
          y.val(val);
        }
      },
      setXYZ : function(val){
        var setVal = function(field , value){
          if(field[0] != document.activeElement){
            field.val(value);
          }
        }

        if(val != null){
          setVal(this.x , val.x);
          setVal(this.y , val.y);
          setVal(this.z , val.z);
        }
        else {
          setVal(this.x , "");
          setVal(this.y , "");
          setVal(this.z , "");
        }
      },
      onChange : onChangeValue,
    };

    vector3dUi.x.change(function() {
      vector3dUi.onChange(vector3dUi);
    });

    vector3dUi.y.change(function() {
      vector3dUi.onChange(vector3dUi);
    });

    vector3dUi.z.change(function() {
      vector3dUi.onChange(vector3dUi);
    });

    return vector3dUi;
  }

  positionUi = ConnectUi({
    x : "#positionX" ,
    y : "#positionY" ,
    z : "#positionZ"},
    OnChangeTransformValue);

  rotationUi = ConnectUi({
    x : "#rotationX" ,
    y : "#rotationY" ,
    z : "#rotationZ"},
    OnChangeTransformValue);

  scaleUi = ConnectUi({
    x : "#scaleX" ,
    y : "#scaleY" ,
    z : "#scaleZ"},
    OnChangeTransformValue);
}

function OnChangeEnableGameobject(){
  var type = {
    type : "changeEnableGameObject",
    id : idGameObject,
    activeSelf : GetActiveSelf(),
  };

  var url = CreateAction(type);

  var xhr = new XMLHttpRequest();
  xhr.open('GET', url, true);
  xhr.responseType = 'json';
  xhr.send();
  xhr.onload = function() {
      ReloadTree();
  }
}

function OnChangeTransformValue(elementUi){

  var type = {
    type : "changeTransform",
    id : idGameObject,
    activeSelf : GetActiveSelf(),
    x : elementUi.x.val(),
    y : elementUi.y.val(),
    z : elementUi.z.val(),
  };

  if(elementUi == positionUi){
    type.transformType = "position";
  }
  else if(elementUi == rotationUi){
    type.transformType = "rotation";
  }
  else if(elementUi == scaleUi){
    type.transformType = "scale";
  }

  var url = CreateAction(type);

  var xhr = new XMLHttpRequest();
  xhr.open('GET', url, true);
  xhr.responseType = 'json';
  xhr.send();
  xhr.onload = function() {

  }
}

function SetValuesComponents(data){
  try {
    if(data != null){

      $("#nameGameObject").text(data.name);
      SetActiveSelf(data.activeSelf);
      positionUi.setXYZ(data.position);
      rotationUi.setXYZ(data.rotation);
      scaleUi.setXYZ(data.scale);
    }
    else {
      $("#nameGameObject").text("No object selected.");
      SetActiveSelf(false);
      positionUi.setXYZ(null);
      rotationUi.setXYZ(null);
      scaleUi.setXYZ(null);
    }

  } catch (e) {

  }
}
