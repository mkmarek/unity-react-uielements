import React from 'react';

const ComponentWithHeader = ({ header, component }) => <>
    {header}
    {component}
</>

export default function ComponentPreview() {
    return (
        <scrollView style={{ backgroundColor: '#173e54' }}>
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
            <listView />

            MinMaxSlider:
            <minMaxSlider />

            PopupWindow:
            <popupWindow />

            Repeat button:
            <repeatButton />

            Scroller:
            <scroller />

            Slider:
            <slider />

            SliderInt:
            <sliderInt />

            TemplateContainer:
            <templateContainer />

            Textfield:
            <textField />

            Toggle:
            <toggle />
        </scrollView>
    )
}