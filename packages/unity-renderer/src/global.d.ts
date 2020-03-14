import Bridge from "./native-to-js-bridge";

declare global {
    interface Natives {
        nativeToJsBridge: (message: string) => void;
        jsToNativeBridge: (message: string) => void;
    }

    namespace NodeJS {
        interface Global {
            bridge: Bridge;
            natives: Natives;
        }
    }
}