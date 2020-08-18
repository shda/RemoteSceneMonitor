let timerId;

function ShowDataToGameObjectById(id){

  clearTimeout(timerId);
  timerId = setTimeout(function tick() {

    utils.getJSON("/gameObjectInfo?" + CreateParams({id : id}) , function(err , data){
      if(err === null){
        SetValuesComponents(data);
      }

    timerId = setTimeout(tick, 500); // (*)
  }, 500);
  });
}


function SetValuesComponents(data){
  try {
    $("#positionX").val(data.position.x);
    $("#positionY").val(data.position.y);
    $("#positionZ").val(data.position.z);

    $("#rotationX").val(data.rotation.x);
    $("#rotationY").val(data.rotation.y);
    $("#rotationZ").val(data.rotation.z);

    $("#scaleX").val(data.scale.x);
    $("#scaleY").val(data.scale.y);
    $("#scaleZ").val(data.scale.z);
  } catch (e) {

  }
}
