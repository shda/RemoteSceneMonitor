var zTree;
var demoIframe;

var setting = {
  view: {
    dblClickExpand: false,
    showLine: true,
    selectedMulti: false,

    fontCss: getFont,
    nameIsHTML: true
  },
  data: {
    simpleData: {
      enable: true,
      idKey: "id",
      pIdKey: "pId",
      rootPId: ""
    }
  },
  edit: {
    enable: true,
    showRemoveBtn: showRemoveBtn,
    showRenameBtn: false,
    drag: {
      isCopy: false,
      isMove: true,
      prev: true,
      iner: true,
      next: true,
    }
  },
  callback: {
    beforeClick: beforeClick,
    beforeDrag: beforeDrag,
    beforeDrop: beforeDrop,

    onRemove: onRemove,

    onCollapse: onCollapse,
    onExpand: onExpand,
  }
};

var openNodes = new Set();

function getFont(treeId, node) {
  return node.font ? node.font : {};
}

function onCollapse(event, treeId, treeNode) {
  openNodes.delete(treeNode.id);
}

function showRemoveBtn(treeId, treeNode){
    return treeNode.drag;
}

function onExpand(event, treeId, treeNode) {
    openNodes.add(treeNode.id);
}

function beforeClick(treeId, treeNode)
{
  var zTree = $.fn.zTree.getZTreeObj("tree");
  if (treeNode.isParent) {
    ShowDataToGameObjectById(treeNode.id);
    return false;
  } else {
    ShowDataToGameObjectById(treeNode.id);
    return true;
  }
}

function onRemove(e, treeId, treeNode) {

  var url = CreateAction({
    type : "delete",
    id : treeNode.id
  });

  var xhr = new XMLHttpRequest();
  xhr.open('GET', url);
  xhr.send();
  xhr.onload = function() {
    ReloadTree();
  }
}

function CreateAction(dataParams){
  return "/action?" + CreateParams(dataParams);
}

function CreateParams(dataParams){
  return new URLSearchParams(dataParams);
}

function beforeDrag(treeId, treeNodes) {
  for (var i=0,l=treeNodes.length; i<l; i++) {
    if (treeNodes[i].drag === false) {
      return false;
    }
  }
  return true;
}
function beforeDrop(treeId, treeNodes, targetNode, moveType) {
  moveNode(treeNodes[0].id,targetNode.id);
  return targetNode ? targetNode.drop !== false : true;
}

function moveNode(idDragNode , idTargetNode){

  var url = CreateAction({
    type : "move",
    idSource : idDragNode,
    idDestination : idTargetNode,
  });

  var xhr = new XMLHttpRequest();
  xhr.open('GET', url, true);
  xhr.responseType = 'json';
  xhr.send();
  xhr.onload = function() {
    ReloadTree();
  }
}

function ReloadTree(){
  utils.getJSON("/json/hierarchy" , function(err , data){
    if(err === null){
      var nodes = CreateHtmlTree(data.scenesRootNodesList);
      var t = $("#tree");
      t = $.fn.zTree.init(t, setting, nodes);
    }
  });
}

function onClickButton(){
  ReloadTree();
}

function CreateHtmlTree(scenesNode){
  var listRootSceneNodes = [];
  for (var i = 0; i < scenesNode.length; i++) {
    var sceneNodeRoot = scenesNode[i];
    var node = CreateNode(sceneNodeRoot);
    CreateHtmlNode(node , sceneNodeRoot.children);
    listRootSceneNodes.push(node);
  }

  return listRootSceneNodes;
}

function CreateHtmlNode(node , children){
  if(children == null)
    return ;
  for (var i = 0; i < children.length; i++) {
    var child =  children[i];
    var nodeChild = CreateNode(child);
    node.children.push(nodeChild);
    if(child.children == null){
      nodeChild.children = null;
    }
    else {
      CreateHtmlNode(nodeChild , child.children);
    }
  }
}

function CreateNode(child){
  var node = {
    name: child.name,
    id: child.id ,
    children: [],
    font:{
      'color': child.isEnable ? '#000000ff' : '#00000055'
    } ,
    drag: !child.isScene,
    open: openNodes.has(child.id)
  };

  if(child.isScene){
    node.icon = "images/scene_icon.png";
  }
  else{
    node.icon = "images/object_icon.png";
  }

  return node;
}
