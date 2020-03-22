import React from 'react';
import Panel from './panel';

const backdropStyles = {
    position: 'absolute',
    left: 0,
    top: 0,
    width: '100%',
    height: '100%',
    backgroundColor: '#000000aa',
    justifyContent: 'center'
}

const panelOuterStyle = {
    width: 400,
    height: 300,
    alignSelf: 'center',
    verticalAlign: 'center'
}

const panelInnerStyle = {
    flexDirection: 'row'
}

const buttonStyle = {
    alignSelf: 'flex-end',
    width: 100,
    marginBottom: 15,
    marginLeft: 15
};

export default function Popup({ onClose }) {

    return <element style={backdropStyles}>
        <Panel outerStyle={panelOuterStyle} innerStyle={panelInnerStyle}>
            <button style={buttonStyle} onClick={onClose} text="Close" />
        </Panel>
    </element>
}