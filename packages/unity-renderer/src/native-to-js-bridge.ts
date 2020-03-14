import ContainerElement from "./components/container-element";
import Element from './components/element';

export interface IBridgePayload {
    messages: IBridgeMessage[]
}

export interface IBridgeMessage {
    operation: string;
    componentId: string;
    callbackName: string;
    hookId: string;
    data: any;
}


export default class Bridge {

    container: ContainerElement;
    componentDataHooks: { [key: string]: (data) => void };
    timeoutHandler: any;
    batchedMessages: any[];

    constructor(container: ContainerElement) {
        this.container = container;
        this.componentDataHooks = {};
        this.batchedMessages = [];
    }

    onMessage(message: string) {
        const payload: IBridgePayload = JSON.parse(message);
        
        for (let message of payload.messages) {
            switch (message.operation) {
                case "event-callback": this.invokeEventCallback(message.componentId, message.callbackName, message.data); break;
                case "update-component-data-hook":  this.invokeComponentDataHook(message.hookId, message.data); break;
                default: console.error(`Unknown operation ${message.operation}`);
            }
        }
    }

    sendBatched(message: any) {
        if (!this.timeoutHandler) {
            this.timeoutHandler = setTimeout(() => {
                global.natives.jsToNativeBridge(JSON.stringify({ messages: this.batchedMessages }));
                this.batchedMessages = [];
                this.timeoutHandler = null;
            }, 25);
        }

        this.batchedMessages.push(message);
    }

    registerComponentDataHook(id: string, queryName: string, setter: (data) => void) {
        this.sendBatched({
            operation: 'register-component-query',
            id,
            queryName
        });

        this.componentDataHooks[id] = setter;
    }

    removeComponentDataHook(id: string) {
        this.sendBatched({
            operation: 'remove-component-query',
            id
        });

        this.componentDataHooks[id] = null;
    }

    isComponentDataHookRegistered(id: string): boolean {
        return !!this.componentDataHooks[id];
    }

    invokeComponentDataHook(id: string, data: any) {
        if (this.componentDataHooks[id]) {
            this.componentDataHooks[id](JSON.parse(data));
        }
    }

    updateComponentDataViaHook(hookId: string, componentIndex: number, index: number, componentData: any) {
        this.sendBatched({
            operation: 'update-component-data-via-hook',
            id: hookId,
            componentIndex,
            index,
            data: JSON.stringify(componentData)
        });
    }

    invokeEventCallback(componentId: string, callbackName: string, data: string) {
        let queue: Element[] = [this.container];
        let eventData = data && JSON.parse(data);

        while (queue.length > 0) {
            const item = queue.pop();
            if (item.id === componentId) {
                item.props[callbackName] && item.props[callbackName](eventData);
                return true;
            }

            for (let child of item.children) queue.push(child);
        }
    }
}