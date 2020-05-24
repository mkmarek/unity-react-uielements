import React from "react";
import { render } from "unity-renderer";

const buttonStyle = {
  height: "64px",
  backgroundColor: "#ffffff",

  borderLeftColor: "#eeeeee",

  borderTopWidth: 0,
  borderLeftWidth: 5,
  borderRightWidth: 0,
  borderBottomWidth: 0,

  marginBottom: 10,

  fontSize: 24,
  unityTextAlign: "middle-left",
  paddingLeft: 46
};

function MenuButton({ text, onMouseUpEvent }) {
  const [isHovered, setIsHovered] = React.useState(false);

  const style = Object.assign(
    {},
    buttonStyle,
    isHovered
      ? {
          backgroundColor: "#ffffff",
          borderLeftColor: "#4175d4"
        }
      : {}
  );

  return (
    <button
      text={text}
      onMouseUpEvent={onMouseUpEvent}
      onMouseOver={() => setIsHovered(true)}
      onMouseOut={() => setIsHovered(false)}
      style={style}
    />
  );
}

function MainMenu({ onSelectOption }) {
  return (
    <element
      style={{
        width: "400px",
        marginLeft: "5%",
        height: "100%",
        justifyContent: "center"
      }}
    >
      <element style={{ fontSize: 32, marginBottom: "20px" }}>
        The best game ever
      </element>
      <MenuButton text="New game" />
      <MenuButton text="Load game" />
      <MenuButton onMouseUpEvent={() => onSelectOption("options")} text="Options" />
      <MenuButton text="Quit" />
    </element>
  );
}

function Options({ onSelectOption }) {
  return (
    <element
      style={{
        width: "400px",
        marginLeft: "5%",
        height: "100%",
        justifyContent: "center"
      }}
    >
      <element style={{ fontSize: 32, marginBottom: "20px" }}>TODO</element>
      <MenuButton onMouseUpEvent={() => onSelectOption("main")} text="Back" />
    </element>
  );
}

function App() {
  const [phase, setPhase] = React.useState("main");

  return (
    <element style={{ width: "100%", height: "100%" }}>
      {phase === "main" && <MainMenu onSelectOption={setPhase} />}
      {phase === "options" && <Options onSelectOption={setPhase} />}
    </element>
  );
}

render(<App />);
