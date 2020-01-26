import ContainerElement from "../components/container-element";
import Element from "../components/element";

export const supportsPersistence = true;

export const cloneInstance = (
    instance: Element,
    updatePayload: any,
    type: string,
    oldProps: any,
    newProps: any,
    internalInstanceHandle: any,
    keepChildren: boolean,
    recyclableInstance: boolean,
) => {
    if (!keepChildren) {
        for (let child of instance.children)
        instance.removeChild(child)
    }

    if (updatePayload) {
        instance.updateProps(updatePayload.props, updatePayload.shouldSendUpdate);
    }

    return instance;
};

export const createContainerChildSet = (container: ContainerElement) => {
    return [];
};

export const appendChildToContainerChildSet = (childSet: Element[], child: Element) => {
    childSet.push(child);
};

export const finalizeContainerChildren = (container: ContainerElement, newChildren: Element[]) => {
    // for (let child of newChildren) {
    //     container.addChild(child);
    // }
};

export const replaceContainerChildren = (container: ContainerElement, newChildren: Element[]) => {
    const toRemove = [];
    const toAdd = [];

    for (let child of container.children) {
        if (!newChildren.find(e => e.id === child.id)) toRemove.push(child);
    }

    for (let child of newChildren) {
        if (!container.children.find(e => e.id === child.id)) toAdd.push(child);
    }

    for (let child of toRemove) container.removeChild(child);
    for (let child of toAdd) container.insertAtIndex(child, newChildren.indexOf(child));
};

export const cloneHiddenInstance = (
    instance: Element,
    type: string,
    props: any,
    internalInstanceHandle: Object,
) => {
    console.log('cloneHiddenInstance')
};

export const cloneUnhiddenInstance = (
    instance: Element,
    type: string,
    props: any,
    internalInstanceHandle: Object,
) => {
    console.log('cloneUnhiddenInstance')
};

export const createHiddenTextInstance = (container: ContainerElement, newChildren) => {
    console.log('createHiddenTextInstance')
};