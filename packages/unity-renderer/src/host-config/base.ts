import ContainerElement from "../components/container-element";
import Element from "../components/element";

const supportedElements = [
    'element',
    'box',
    'button',
    'foldout',
    'image',
    'label',
    'listview',
    'minmaxslider',
    'popupwindow',
    'repeatbutton',
    'scroller',
    'scrollview',
    'slider',
    'sliderint',
    'templatecontainer',
    'textfield',
    'toggle'
]

export const noTimeout = -1;
export const now = Date.now;

export const getPublicInstance = (instance: Element) => {
    return instance;
};

export const getRootHostContext = (rootContainerInstance: ContainerElement) => {
    return { };
};

export const getChildHostContext = (parentHostContext, type: string, rootContainerInstance: ContainerElement) => {
    return parentHostContext;
};

export const prepareForCommit = (containerInfo: ContainerElement) => {

};

export const resetAfterCommit = (containerInfo: ContainerElement) => {
    containerInfo.commitUpdates();
};

export const createInstance = (
    type: string,
    props: any,
    rootContainerInstance: ContainerElement,
    hostContext: any,
    internalInstanceHandle: any,
): Element => {
    if (supportedElements.includes(type)) return new Element(type, props, rootContainerInstance);
    throw new Error(`Unknown type: ${type}`);
};

export const appendInitialChild = (parentInstance: Element, child: Element) => {
    parentInstance.addChild(child)
};

export const finalizeInitialChildren = (
    parentInstance: Element,
    type: string,
    props: any,
    rootContainerInstance: ContainerElement,
    hostContext: any,
) => {
    return true;
};

export const prepareUpdate = (
    instance: Element,
    type: string,
    oldProps: any,
    newProps: any,
    rootContainerInstance: ContainerElement,
    hostContext: any,
) => {

    const oldP = { ...oldProps, children: null };
    const newP = { ...newProps, children: null };

    return {
        shouldSendUpdate: JSON.stringify(oldP) !== JSON.stringify(newP),
        props: newProps
    };
};

export const shouldSetTextContent = (type: string, props: any) => type === 'text';

export const shouldDeprioritizeSubtree = (type: string, props: any) => false;

export const createTextInstance = (
    text: string,
    rootContainerInstance: ContainerElement,
    hostContext: any,
    internalInstanceHandle: any,
) => {
    return new Element('text', { text }, rootContainerInstance);
};

export const scheduleDeferredCallback = (
    callback: () => any,
    options?: { timeout: number },
) => {
    return setTimeout(callback, options ? options.timeout : 1);
};

export const cancelDeferredCallback = (callbackID: number) => {
    clearTimeout(callbackID);
};

export const setTimeout = (handler: (...args: any[]) => void, timeout: number) => {
    return setTimeout(handler, timeout);
};

export const clearTimeout = (handle: number) => {
    clearTimeout(handle);
};