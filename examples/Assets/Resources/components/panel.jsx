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

export default function Panel({ innerStyle, outerStyle, children }) {

    outerStyle = Object.assign(borderColor('#59b6cc'), borderWidth('1'), {
        width: '100%',
        height: '100%',
    }, outerStyle);

    const midBorderStyle = Object.assign(borderColor('#0b2431'), borderWidth('1'), {
        width: '100%',
        height: '100%'
    });

    const innerBorderStyle = Object.assign(borderColor('#2d5369'), borderWidth('1'), {
        width: '100%',
        height: '100%',
        backgroundColor: '#173e54',
    }, innerStyle);

    return (
        <element style={outerStyle}>
            <element style={midBorderStyle}>
                <element style={innerBorderStyle}>
                    {children}
                </element>
            </element>
        </element>
    );
}