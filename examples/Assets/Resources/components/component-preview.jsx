import React from 'react';

const ComponentWithHeader = ({ header, component }) => <>
    {header}
    {component}
</>

export default function ComponentPreview() {
    return (
        <scrollview style={{ backgroundColor: '#173e54' }}>
            <ComponentWithHeader header="Box" component={
                <box style={{ color: '#000000' }}>
                    Some content in the box
                </box>
            }/>

            Button:
            <button text="Some button text" />

            Foldout:
            <foldout />

            Image:
            <image image="images/unity-logo" scaleMode="scale-to-fit" tintColor="#aa00bb" />

            Label:
            <label />

            ListView:
            <listview />

            MinMaxSlider:
            <minmaxslider />

            PopupWindow:
            <popupwindow />

            Repeat button:
            <repeatbutton />

            Scroller:
            <scroller />

            Slider:
            <slider />

            SliderInt:
            <sliderint />

            TemplateContainer:
            <templatecontainer />

            Textfield:
            <textfield />

            Toggle:
            <toggle />
        </scrollview>
    )
}