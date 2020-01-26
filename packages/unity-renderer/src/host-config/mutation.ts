import ContainerElement from "../components/container-element";
import Element from "../components/element";

export const supportsMutation = true;

export const appendChild = (parentInstance: Element, child: Element): void => {
    parentInstance.addChild(child);
};

export const appendChildToContainer = (container: ContainerElement, child: Element): void => {
    container.addChild(child);
};

export const commitTextUpdate = (textInstance: Element, oldText: string, newText: string): void => {
    if (oldText !== newText) {
        textInstance.updateProps({ text: newText });
    }
};

export const commitMount = (
    instance: Element,
    type: string,
    newProps: any,
    internalInstanceHandle: any,
): void => {
    
};

export const commitUpdate = (
    instance: Element,
    updatePayload: any,
    type: string,
    oldProps: any,
    newProps: any,
    internalInstanceHandle: any,
): any => {
    instance.updateProps(updatePayload.props, updatePayload.shouldSendUpdate);

    return updatePayload.props;
};

export const insertBefore = (parentInstance: Element, child: Element, beforeChild: Element): void => {
    parentInstance.insertBefore(child, beforeChild);
};

export const insertInContainerBefore = (
    container: ContainerElement,
    child: Element,
    beforeChild: Element,
): void => {
    container.insertBefore(child, beforeChild);
};

export const removeChild = (parentInstance: Element, child: Element): void => {
    parentInstance.removeChild(child);
};

export const removeChildFromContainer = (container: Element, child: Element): void => {
    container.removeChild(child);
};

export const resetTextContent = (instance: Element) => {

};

export const hideInstance = (instance: Element) => {

};

export const hideTextInstance = (instance: Element) => {

};

export const unhideInstance = (instance: Element) => {

};

export const unhideTextInstance = (instance: Element) => {

};