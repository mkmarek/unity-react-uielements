import React from 'react';

export default function ComponentPreview() {
    return (
        <scrollview style={{ backgroundColor: '#173e54' }}>
            Box:
            <box style={{ color: '#000000' }}>
                Some content in the box
            </box>

            Button:
            <button text="Some button text" />

            Foldout:
            <foldout />

            Image:
            <image />

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