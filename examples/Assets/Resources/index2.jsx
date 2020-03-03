import React from "react";
import { render } from "unity-renderer";

const buttonStyle = {
  height: "64px",
  backgroundColor: "#ffffff",

  borderTopColor: "#000000",
  borderLeftColor: "#4175d4",
  borderRightColor: "#000000",
  borderBottomColor: "#4175d4",

  borderTopWidth: 0,
  borderLeftWidth: 1,
  borderRightWidth: 0,
  borderBottomWidth: 0,

  marginBottom: 10,

  fontSize: 24,
  unityTextAlign: "middle-left",
  paddingLeft: 50
};

function MenuButton({ text, onClick }) {
  const [isHovered, setIsHovered] = React.useState(false);

  const style = Object.assign(
    {},
    buttonStyle,
    isHovered
      ? {
          backgroundColor: "#eeeeee",
          borderLeftColor: "#4cb825",
          borderBottomColor: "#4cb825",
          borderLeftWidth: 5,
          paddingLeft: 46,
          borderBottomWidth: 0
        }
      : {}
  );

  return (
    <button
      text={text}
      onClick={onClick}
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
      <MenuButton onClick={() => onSelectOption("options")} text="Options" />
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
      <MenuButton onClick={() => onSelectOption("main")} text="Back" />
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
