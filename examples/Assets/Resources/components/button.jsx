import React from 'react';

const borderWidth = (width) => ({
    borderTopWidth: width,
    borderLeftWidth: width,
    borderBottomWidth: width,
    borderRightWidth: width
});

const borderColor = (color) => ({
    borderTopColor: color,
    borderLeftColor: color,
    borderBottomColor: color,
    borderRightColor: color
});

export default function Button({ style, innerStyle={}, onMouseUpEvent, children }) {
    const outerStyle = Object.assign(borderColor('#0b2431'), borderWidth('1'),{
        width: '100%',
        height: '100%',
    }, style);

    const innerBorderStyle = Object.assign(borderColor('#2d5369'), borderWidth('1'), {
        width: '100%',
        height: '100%',
        backgroundColor: '#173e54',
        alignItems: "Center",
        alignContent: "Center",
        justifyContent: "Center",
        flexDirection: "Column",
        unityTextAlign: 'MiddleCenter',
    }, innerStyle);

    return (
        <visualElement style={outerStyle}>
           <visualElement onMouseUpEvent={onMouseUpEvent} style={innerBorderStyle}>
                {children}
            </visualElement>
        </visualElement>
    );
}