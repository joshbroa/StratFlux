var nodeList;
var nodeBeingDraggedName;

const nodeTypeReference = {
    'ValueNodes': ['Constant Value', 'Open Value', 'Low Value', 'High Value', 'Close Value', 'Volume Value', 'SMA', 'EMA', 'BOLU', 'BOLD', 'RSI', 'MFI'],
    'OperationNodes': ['Add', 'Subtract', 'Multiply', 'Divide'],
    'ConditionNodes': ['Equal to', 'Less than', 'Less than or equal to', 'Greater than', 'Greater than or equal to', 'Crossover', 'Crossunder'],
    'ConditionalOperatorNodes': ['And', 'Or', 'Xor', 'Not'],
    'ActionNodes': ['Take Profit/Stop Loss (Percentage)', 'Take Profit/Stop Loss (Absolute)', 'Increase Position', 'Decrease Position', 'Set Position', 'Reset Position', 'Close All Active Trades'],
    'IndicatorNodes': ['Indicator with Stock Data', 'Indicator without Stock Data', 'Enclosed Area Indicator with Stock Data', 'Enclosed Area Indicator without Stock Data'],
    'ChartNodes': ['Chart']
};

// This function fills the node list will the available nodes depending on which type is selected
function displayAvailableNodes(type) {
    nodeList.innerHTML = "";

    let nodesToDisplay = nodeTypeReference[type];

    nodesToDisplay.forEach(node => {
        const nodeLi = document.createElement('li');
        nodeLi.className = "node-item";
        nodeLi.innerHTML = node;

        nodeLi.setAttribute('draggable', true);
        nodeLi.setAttribute('ondragstart', `nodeDrag(\'${node}\')`);

        nodeList.appendChild(nodeLi);
    });
}

// This function gets a list of all nodes of the currently selected types
function getSelectedTypes() {
    const typeLis = document.getElementsByClassName('node-type-item');
    var selectedLis = [];

    for (var i = 0; i < typeLis.length; i++) {
        if (typeLis[i].classList.contains('node-type-item-selected')) {
            selectedLis.push(typeLis[i]);
        }
    }

    return selectedLis;
}

// This function is called when a new node type is selected to be displayed
function typeSelected(type, typeLi) {

    getSelectedTypes().forEach(typeListItem => {
        typeListItem.classList.remove('node-type-item-selected');
    });

    typeLi.classList.add('node-type-item-selected')

    displayAvailableNodes(type)
}

// This function is used to keep track of which nodes are being dragged onto the drawflow canvas
function nodeDrag(nodeName) {
    nodeBeingDraggedName = nodeName;
}

// This function is called when the page has loaded
document.addEventListener('DOMContentLoaded', () => {
    // Asp form and json input field retrieved for the saving functionality
    const form = document.getElementsByClassName('strategy-save-form')[0];
    const jsonInputField = document.getElementById('jsonDataElement');

    // When strategy is saved, form is submitted and JSON data will be exported to the JSON input field
    form.addEventListener('submit', event => {
        // Default submission is stopped and manually handled from this point so that the form is not automatically submitted without updating the JSON
        event.preventDefault();

        // If the form isn't valid, then there is no point trying to save it anyway, so the function returns
        if (!$(form).valid()) { return; }

        // Retrieve drawflow data in JSON and then convert to string
        var drawflowData = editor.export();
        var drawflowJsonString = JSON.stringify(drawflowData);

        // Input field is updated to contain most recent data and then the form is submitted
        jsonInputField.value = drawflowJsonString;

        // Submit form manually
        form.submit()
    });

    // Set up initial drawflow canvas
    var id = document.getElementById('drawflow');

    const editor = new Drawflow(id);

    editor.reroute = true;

    editor.start();

    // If the JSON input field contains any data once page has loaded, import it into the drawflow canvas
    if (jsonInputField.value) {
        editor.import(JSON.parse(jsonInputField.value));
    }

    nodeList = document.getElementsByClassName('node-list')[0];

    displayAvailableNodes('ValueNodes', nodeList);

    // This function is used for adding a new node to the canvas when a user drags and drops a certain node from the displayed list of available nodes
    function addNodeToDrawFlow(nodeName, posX, posY) {
        if (!nodes[nodeName]) {
            console.log('This node currently has no implentation.');
            return;
        }

        const node = nodes[nodeName];

        // This position calculation was given in the drawflow documentation
        // It works out where in the editor to place the new node based on the position of the user's cursor on the page
        posX = posX * (editor.precanvas.clientWidth / (editor.precanvas.clientWidth * editor.zoom)) - (editor.precanvas.getBoundingClientRect().x * (editor.precanvas.clientWidth / (editor.precanvas.clientWidth * editor.zoom)));
        posY = posY * (editor.precanvas.clientHeight / (editor.precanvas.clientHeight * editor.zoom)) - (editor.precanvas.getBoundingClientRect().y * (editor.precanvas.clientHeight / (editor.precanvas.clientHeight * editor.zoom)));

        editor.addNode(nodeName, node.inputs, node.outputs, posX, posY, node.class, node.data, node.html);
    }

    // This function is called when a certain node is dropped onto the canvas
    window.nodeDrop = function(event) {
        event.preventDefault();

        addNodeToDrawFlow(nodeBeingDraggedName, event.clientX, event.clientY);

        nodeBeingDraggedName = null;
    }

    // This function updates the grid size, opacity and position when panning and zooming
    function updateGrid(zoom, position) {
        var root = document.documentElement;

        var newSize = `${50 * zoom}px`;

        var newPosition = `${position.x}px ${position.y}px`;

        var newGridOpacity = `linear-gradient(to right, rgba(0, 0, 0, ${1 * zoom}) 1px, transparent 1px), linear-gradient(to bottom, rgba(0, 0, 0, ${1 * zoom}) 1px, transparent 1px)`;

        // Change the css properties of the drawflow theme
        root.style.setProperty('--dfBackgroundSize', newSize);
        root.style.setProperty('--dfBackgroundPosition', newPosition);
        root.style.setProperty('--dfBackgroundImage', newGridOpacity);
    }

    // This function is called when a zoom event on the canvas takes place
    editor.on('zoom', zoom => {
        updateGrid(zoom, { x: editor.canvas_x, y: editor.canvas_y });
    });

    // This function is called when the position of the canvas is updated by the user dragging the background
    editor.on('translate', position => {
        const editorRect = id.getBoundingClientRect();

        // Calculate new position from center to be consistent with zooming
        updateGrid(editor.zoom, { x: position.x + (editorRect.width / 2), y: position.y + (editorRect.height / 2) });
    });

    // This function zooms in and out while cursor is over the drawflow editor
    id.addEventListener('wheel', event => {
        event.preventDefault();

        if (event.deltaY < 0) {
            editor.zoom_in();
        }
        else {
            editor.zoom_out();
        }

        const editorRect = id.getBoundingClientRect();

        updateGrid(editor.zoom, { x: editor.canvas_x + (editorRect.width / 2), y: editor.canvas_y + (editorRect.height / 2) });
    });

    // Variable for storing the id of the node which data has most recently been changed
    var recentDataChangeNodeId;

    // The variable is assigned when data is changed
    editor.on('nodeDataChanged', id => recentDataChangeNodeId = id);

    // When the node is unselected, it's drawflow data will be updated so that any changes will be included in the export when saved
    editor.on('nodeUnselected', unselected => {
        if (recentDataChangeNodeId != null) {
            try {
                var newData = editor.getNodeFromId(recentDataChangeNodeId).data;
                editor.updateNodeDataFromId(recentDataChangeNodeId, newData);
            }
            catch (error) {

            }
        }
    });

    // This function is called when a connection between nodes is made
    editor.on('connectionCreated', connection => {
        var fromNode = editor.getNodeFromId(connection.output_id);
        var toNode = editor.getNodeFromId(connection.input_id);

        // Here, for each node type is a list of the other node types it can be connected to
        const allowedConnectionReference = {
            'value-node': ['operation-node', 'condition-node', 'indicator-node'],
            'operation-node': ['operation-node', 'condition-node', 'indicator-node'],
            'condition-node': ['conditional-operator-node', 'action-node'],
            'conditional-operator-node': ['conditional-operator-node', 'action-node'],
            'action-node': [],
            'indicator-node': ['chart-node'],
            'chart-node': []
        }

        // If the connection is not a valid connection due to being the wrong types, it is removed
        // There is an exception for the take profit/stop loss nodes because their inputs have different node-types depending on the input class
        if (toNode.data['node_name'] === 'TPSLP' || toNode.data['node_name'] === 'TPSLA') {
            if (connection.input_class === 'input_1') {
                // If the connection is for input 1 of the take profit/stop loss node, then the allowed connection reference can be used
                if (!allowedConnectionReference[fromNode.data['node_type']].includes(toNode.data['node_type'])) {
                    editor.removeSingleConnection(connection.output_id, connection.input_id, connection.output_class, connection.input_class);
                    return;
                }
            }
            else {
                // For any input except input_1 for the take profit/stop loss node, only value and operation nodes are allowed as input
                if (!(fromNode.data['node_type'] === 'value-node' || fromNode.data['node_type'] === 'operation-node')) {
                    editor.removeSingleConnection(connection.output_id, connection.input_id, connection.output_class, connection.input_class);
                    return;
                }
            }
        }
        else if (!allowedConnectionReference[fromNode.data['node_type']].includes(toNode.data['node_type'])) {
            // For all other nodes, if they are not in the allowed connection referece, the connection will be deleted
            editor.removeSingleConnection(connection.output_id, connection.input_id, connection.output_class, connection.input_class);
            return;
        }

        // Chart nodes may have multiple inputs to the same output so only non-chart types apply to the condition
        if (toNode.data['node_type'] != 'chart-node') {
            // For any other connections which use the new connection's input, they are removed so that only one connection to the same input is in place
            toNode.inputs[connection.input_class].connections.forEach(otherConnection => {
                if (otherConnection.node != connection.output_id) {
                    editor.removeSingleConnection(otherConnection.node, connection.input_id, otherConnection.input, connection.input_class);
                }
            });
        }
    });
});


// This is a dictionary of all possible nodes so that they can be easily instantiated
nodes = {
    'Constant Value': {
        inputs: 0,
        outputs: 1,
        class: 'value-type const-value-node',
        data: {
            'node_name': 'CONST',
            'node_type': 'value-node',
            'value': 0
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">Constant Value</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = <input class="node-input-box" type="number" df-value></input>
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'Open Value': {
        inputs: 0,
        outputs: 1,
        class: 'value-type open-value-node',
        data: {
            'node_name': 'OPEN',
            'node_type': 'value-node',
            'tick_offset': 0
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">Open Value</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Open Value from <input class="node-input-box-integer" type="number" min="0" max="50" df-tick_offset></input> tick(s) ago
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'Low Value': {
        inputs: 0,
        outputs: 1,
        class: 'value-type low-value-node',
        data: {
            'node_name': 'LOW',
            'node_type': 'value-node',
            'tick_offset': 0
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">Low Value</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Low Value from <input class="node-input-box-integer" type="number" min="0" max="50" df-tick_offset></input> tick(s) ago
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'High Value': {
        inputs: 0,
        outputs: 1,
        class: 'value-type high-value-node',
        data: {
            'node_name': 'HIGH',
            'node_type': 'value-node',
            'tick_offset': 0
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">High Value</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = High Value from <input class="node-input-box-integer" type="number" min="0" max="50" df-tick_offset></input> tick(s) ago
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'Close Value': {
        inputs: 0,
        outputs: 1,
        class: 'value-type close-value-node',
        data: {
            'node_name': 'CLOSE',
            'node_type': 'value-node',
            'tick_offset': 0
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">Close Value</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Close Value from <input class="node-input-box-integer" type="number" min="0" max="50" df-tick_offset></input> tick(s) ago
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'Volume Value': {
        inputs: 0,
        outputs: 1,
        class: 'value-type volume-value-node',
        data: {
            'node_name': 'VOLUME',
            'node_type': 'value-node',
            'tick_offset': 0
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">Volume Value</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Volume Value from <input class="node-input-box-integer" type="number" min="0" max="50" df-tick_offset></input> tick(s) ago
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'SMA': {
        inputs: 0,
        outputs: 1,
        class: 'value-type sma-node',
        data: {
            'node_name': 'SMA',
            'node_type': 'value-node',
            'lookback_period': 14
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">SMA</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Simple Moving Average with lookback period of <input class="node-input-box-integer" type="number" min="1" max="50" df-lookback_period></input> tick(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'EMA': {
        inputs: 0,
        outputs: 1,
        class: 'value-type ema-node',
        data: {
            'node_name': 'EMA',
            'node_type': 'value-node',
            'lookback_period': 14
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">EMA</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Exponential Moving Average with lookback period of <input class="node-input-box-integer" type="number" min="1" max="50" df-lookback_period></input> tick(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'BOLU': {
        inputs: 0,
        outputs: 1,
        class: 'value-type bolu-node',
        data: {
            'node_name': 'BOLU',
            'node_type': 'value-node',
            'lookback_period': 14,
            'standard_deviations': 1.5
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">BOLU</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Upper Bollinger Band of <input class="node-input-box" type="number" df-standard_deviations></input> standard deviations over last <input class="node-input-box-integer" type="number" min="1" max="50" df-lookback_period></input> tick(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'BOLD': {
        inputs: 0,
        outputs: 1,
        class: 'value-type bold-node',
        data: {
            'node_name': 'BOLD',
            'node_type': 'value-node',
            'lookback_period': 14,
            'standard_deviations': 1.5
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">BOLD</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Lower Bollinger Band of <input class="node-input-box" type="number" df-standard_deviations></input> standard deviations over last <input class="node-input-box-integer" type="number" min="1" max="50" df-lookback_period></input> tick(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'RSI': {
        inputs: 0,
        outputs: 1,
        class: 'value-type rsi-node',
        data: {
            'node_name': 'RSI',
            'node_type': 'value-node',
            'lookback_period': 14
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">RSI</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Relative Strength Index with lookback period of <input class="node-input-box-integer" type="number" min="1" max="50" df-lookback_period></input> tick(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'MFI': {
        inputs: 0,
        outputs: 1,
        class: 'value-type mfi-node',
        data: {
            'node_name': 'MFI',
            'node_type': 'value-node',
            'lookback_period': 14
        },
        html: `<ul class="input-label-list"></ul>
               <div class="center-label-div">
                 <span class="center-label-title">MSI</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     C = Money Flow Index with lookback period of <input class="node-input-box-integer" type="number" min="1" max="50" df-lookback_period></input> tick(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ValueNodes'
    },
    'Add': {
        inputs: 2,
        outputs: 1,
        class: 'operation-type add-node',
        data: {
            'node_name': 'ADD',
            'node_type': 'operation-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Add</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">A + B = C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'OperationNodes'
    },
    'Subtract': {
        inputs: 2,
        outputs: 1,
        class: 'operation-type subtract-node',
        data: {
            'node_name': 'SUBTRACT',
            'node_type': 'operation-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Subtract</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">A - B = C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'OperationNodes'
    },
    'Multiply': {
        inputs: 2,
        outputs: 1,
        class: 'operation-type multiply-node',
        data: {
            'node_name': 'MULTIPLY',
            'node_type': 'operation-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Multiply</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">A * B = C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'OperationNodes'
    },
    'Divide': {
        inputs: 2,
        outputs: 1,
        class: 'operation-type divide-node',
        data: {
            'node_name': 'DIVIDE',
            'node_type': 'operation-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Divide</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">A / B = C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'OperationNodes'
    },
    'Equal to': {
        inputs: 2,
        outputs: 1,
        class: 'condition-type equal-to-node',
        data: {
            'node_name': 'EQUAL',
            'node_type': 'condition-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Equal to</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A = B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionNodes'
    },
    'Less than': {
        inputs: 2,
        outputs: 1,
        class: 'condition-type less-than-node',
        data: {
            'node_name': 'LESS',
            'node_type': 'condition-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li></ul>
               <div class="center-label-div">
                 <span class="center-label-title">Less than</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A < B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionNodes'
    },
    'Less than or equal to': {
        inputs: 2,
        outputs: 1,
        class: 'condition-type less-than-or-equal-to-node',
        data: {
            'node_name': 'LESS-EQUAL',
            'node_type': 'condition-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Less than or equal to</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A ≤ B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionNodes'
    },
    'Greater than': {
        inputs: 2,
        outputs: 1,
        class: 'condition-type greater-than-node',
        data: {
            'node_name': 'GREATER',
            'node_type': 'condition-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Greater than</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A > B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionNodes'
    },
    'Greater than or equal to': {
        inputs: 2,
        outputs: 1,
        class: 'condition-type greater-than-or-equal-to-node',
        data: {
            'node_name': 'GREATER-EQUAL',
            'node_type': 'condition-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Greater than or equal to</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A ≥ B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionNodes'
    },
    'Crossover': {
        inputs: 2,
        outputs: 1,
        class: 'condition-type crossover-node',
        data: {
            'node_name': 'CROSSOVER',
            'node_type': 'condition-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Crossover</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A Crosses Over B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionNodes'
    },
    'Crossunder': {
        inputs: 2,
        outputs: 1,
        class: 'condition-type crossunder-node',
        data: {
            'node_name': 'CROSSUNDER',
            'node_type': 'condition-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Crossunder</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A Crosses Under B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionNodes'
    },
    'And': {
        inputs: 2,
        outputs: 1,
        class: 'conditional-operator-type and-node',
        data: {
            'node_name': 'AND',
            'node_type': 'conditional-operator-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">And</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A AND B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionalOperatorNodes'
    },
    'Or': {
        inputs: 2,
        outputs: 1,
        class: 'conditional-operator-type or-node',
        data: {
            'node_name': 'OR',
            'node_type': 'conditional-operator-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Or</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A OR B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionalOperatorNodes'
    },
    'Xor': {
        inputs: 2,
        outputs: 1,
        class: 'conditional-operator-type xor-node',
        data: {
            'node_name': 'XOR',
            'node_type': 'conditional-operator-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Xor</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">IF (A XOR B) THEN C</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'ConditionalOperatorNodes'
    },
    'Not': {
        inputs: 1,
        outputs: 1,
        class: 'conditional-operator-type not-node',
        data: {
            'node_name': 'NOT',
            'node_type': 'conditional-operator-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Not</span>
                 <div class="center-label-function-div">
                   <span class="center-label-function">NOT A = B</span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">B</li>
               </ul>`,
        type: 'ConditionalOperatorNodes'
    },
    'Take Profit/Stop Loss (Percentage)': {
        inputs: 3,
        outputs: 0,
        class: 'action-type tpslp-node',
        data: {
            'node_name': 'TPSLP',
            'node_type': 'action-node',
            'share_amount': 1
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
                 <li class="input-label">C</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Take Profit/Stop Loss (Percentage)</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     When A is active, buy <input class="node-input-box-integer" type="number" df-share_amount></input> share(s) with a take profit of +B% and stop loss of -C%
                   </div>
                 </div>
               </div>
               <ul class="output-label-list"></ul>`,
        type: 'ActionNodes'
    },
    'Take Profit/Stop Loss (Absolute)': {
        inputs: 3,
        outputs: 0,
        class: 'action-type tpsla-node',
        data: {
            'node_name': 'TPSLA',
            'node_type': 'action-node',
            'share_amount': 1
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
                 <li class="input-label">C</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Take Profit/Stop Loss (Absolute)</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     When A is active, buy <input class="node-input-box-integer" type="number" df-share_amount></input> share(s) with a take profit of +$B and stop loss of -$C
                   </div>
                 </div>
               </div>
               <ul class="output-label-list"></ul>`,
        type: 'ActionNodes'
    },
    'Increase Position': {
        inputs: 1,
        outputs: 0,
        class: 'action-type increase-position-node',
        data: {
            'node_name': 'IPOS',
            'node_type': 'action-node',
            'increase_by': 1
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Increase Position</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     When A is active, increase position by <input class="node-input-box-integer" type="number" min="1" df-increase_by></input> share(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list"></ul>`,
        type: 'ActionNodes'
    },
    'Decrease Position': {
        inputs: 1,
        outputs: 0,
        class: 'action-type decrease-position-node',
        data: {
            'node_name': 'DPOS',
            'node_type': 'action-node',
            'decrease_by': 1
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Decrease Position</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     When A is active, decrease position by <input class="node-input-box-integer" type="number" min="1" df-decrease_by></input> share(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list"></ul>`,
        type: 'ActionNodes'
    },
    'Set Position': {
        inputs: 1,
        outputs: 0,
        class: 'action-type set-position-node',
        data: {
            'node_name': 'SPOS',
            'node_type': 'action-node',
            'position': 0
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Set Position</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     When A is active, set position to <input class="node-input-box-integer" type="number" df-position></input> share(s)
                   </div>
                 </div>
               </div>
               <ul class="output-label-list"></ul>`,
        type: 'ActionNodes'
    },
    'Reset Position': {
        inputs: 1,
        outputs: 0,
        class: 'action-type reset-position-node',
        data: {
            'node_name': 'RPOS',
            'node_type': 'action-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Reset Position</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     When A is active, reset position to 0 shares
                   </div>
                 </div>
               </div>
               <ul class="output-label-list"></ul>`,
        type: 'ActionNodes'
    },
    'Close All Active Trades': {
        inputs: 1,
        outputs: 0,
        class: 'action-type close-trades-node',
        data: {
            'node_name': 'CLOSE-TRADES',
            'node_type': 'action-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Close All Active Trades</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     When A is active, close all active trades
                   </div>
                 </div>
               </div>
               <ul class="output-label-list"></ul>`,
        type: 'ActionNodes'
    },
    'Indicator with Stock Data': {
        inputs: 1,
        outputs: 1,
        class: 'indicator-type indicator-with-stock-data-node',
        data: {
            'node_name': 'INDICATOR-STOCK',
            'node_type': 'indicator-node',
            'indicator_name': 'Enter Indicator Name',
            'primary_line_color': '#11FF11'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Indicator with Stock Data</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     Record values of A for each tick as <input df-indicator_name></input>
                   </div>
                   <div class="center-label-function">
                     Line Colour: <input type="color" df-primary_line_color></input>
                   </div>
                   <span class="center-label-function">
                     Assign B to a chart.
                   </span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">B</li>
               </ul>`,
        type: 'IndicatorNodes'
    },
    'Indicator without Stock Data': {
        inputs: 1,
        outputs: 1,
        class: 'indicator-type indicator-without-stock-data-node',
        data: {
            'node_name': 'INDICATOR',
            'node_type': 'indicator-node',
            'indicator_name': 'Enter Indicator Name',
            'primary_line_color': '#11FF11'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Indicator without Stock Data</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     Record values of A for each tick as <input df-indicator_name></input>
                   </div>
                   <div class="center-label-function">
                     Line Colour: <input type="color" df-primary_line_color></input>
                   </div>
                   <span class="center-label-function">
                     Assign B to a chart.
                   </span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">B</li>
               </ul>`,
        type: 'IndicatorNodes'
    },
    'Enclosed Area Indicator with Stock Data': {
        inputs: 2,
        outputs: 1,
        class: 'indicator-type enclosed-area-indicator-with-stock-data-node',
        data: {
            'node_name': 'AREA-INDICATOR-STOCK',
            'node_type': 'indicator-node',
            'indicator_name': 'Enter Indicator Name',
            'primary_line_color': '#1111FF',
            'secondary_line_color': '#CCCC11',
            'positive_area_color': '#11FF11',
            'negative_area_color': '#FF1111'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Enclosed Area Indicator with Stock Data</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     Record values of A and B for each tick as <input df-indicator_name></input>
                   </div>
                   <div class="center-label-function">
                     Primary Line Colour: <input type="color" df-primary_line_color></input>
                   </div>
                   <div class="center-label-function">
                     Secondary Line Colour: <input type="color" df-secondary_line_color></input>
                   </div>
                   <div class="center-label-function">
                     Positive Area Colour: <input type="color" df-positive_area_color></input>
                   </div>
                   <div class="center-label-function">
                     Negative Area Colour: <input type="color" df-negative_area_color></input>
                   </div>
                   <span class="center-label-function">
                     A is the primary line and B is the secondary line. Assign C to a chart.
                   </span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'IndicatorNodes'
    },
    'Enclosed Area Indicator without Stock Data': {
        inputs: 2,
        outputs: 1,
        class: 'indicator-type enclosed-area-indicator-without-stock-data-node',
        data: {
            'node_name': 'AREA-INDICATOR',
            'node_type': 'indicator-node',
            'indicator_name': 'Enter Indicator Name',
            'primary_line_color': '#1111FF',
            'secondary_line_color': '#CCCC11',
            'positive_area_color': '#11FF11',
            'negative_area_color': '#FF1111'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
                 <li class="input-label">B</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Enclosed Area Indicator without Stock Data</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     Record values of A and B for each tick as <input df-indicator_name></input>
                   </div>
                   <div class="center-label-function">
                     Primary Line Colour: <input type="color" df-primary_line_color></input>
                   </div>
                   <div class="center-label-function">
                     Secondary Line Colour: <input type="color" df-secondary_line_color></input>
                   </div>
                   <div class="center-label-function">
                     Positive Area Colour: <input type="color" df-positive_area_color></input>
                   </div>
                   <div class="center-label-function">
                     Negative Area Colour: <input type="color" df-negative_area_color></input>
                   </div>
                   <span class="center-label-function">
                     A is the primary line and B is the secondary line. Assign C to a chart.
                   </span>
                 </div>
               </div>
               <ul class="output-label-list">
                 <li class="output-label">C</li>
               </ul>`,
        type: 'IndicatorNodes'
    },
    'Chart': {
        inputs: 1,
        outputs: 0,
        class: 'chart-type chart-node',
        data: {
            'node_name': 'CHART',
            'node_type': 'chart-node'
        },
        html: `<ul class="input-label-list">
                 <li class="input-label">A</li>
               </ul>
               <div class="center-label-div">
                 <span class="center-label-title">Chart</span>
                 <div class="center-label-function-div">
                   <div class="center-label-function">
                     All indicators assigned to A will be displayed on this chart.
                   </div>
                 </div>
               </div>
               <ul class="output-label-list"></ul>`,
        type: 'ChartNodes'
    }
}