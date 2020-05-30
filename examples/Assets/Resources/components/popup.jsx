import React from 'react';
import Panel from './panel';

const backdropStyles = {
    position: 'Absolute',
    left: 0,
    top: 0,
    width: '100%',
    height: '100%',
    backgroundColor: '#000000aa',
    justifyContent: 'Center'
}

const panelOuterStyle = {
    width: 400,
    height: 300,
    alignSelf: 'Center',
    verticalAlign: 'Center'
}

const panelInnerStyle = {
    flexDirection: 'Row'
}

const buttonStyle = {
    alignSelf: 'FlexEnd',
    width: 100,
    marginBottom: 15,
    marginLeft: 15
};

export default function Popup({ onClose }) {

    return <visualElement style={backdropStyles}>
        <Panel outerStyle={panelOuterStyle} innerStyle={panelInnerStyle}>
            <button style={buttonStyle} onMouseUpEvent={onClose} text="Close" />
        </Panel>
    </visualElement>
}