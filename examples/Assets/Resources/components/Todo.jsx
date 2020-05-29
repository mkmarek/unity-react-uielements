import React, { useState } from 'react';
import { useQuery } from 'unity-renderer';
import Button from './button';

function TodoItem({ value, onRemove }) {

    return (
        <Button innerStyle={{ flexDirection: 'Row', justifyContent: 'SpaceBetween' }} style={{ height: 70 }}>
            <visualElement style={{ marginLeft: 10 }}>{value}</visualElement>
            <Button onMouseUpEvent={onRemove} style={{ width: 70, height: 50, marginRight: 5 }}>Delete</Button>
        </Button>
    )
}

const todoItemComponentName = 'TodoItemComponent';
const TodoItemComponent = getFactory(todoItemComponentName);

export default function Todo() {
    const [value, setValue] = useState('');
    const query = useQuery([todoItemComponentName]);

    const items = [];
    for (let i = 0; i < query.getSize(); i++) {
        items.push({
            entity: query.getElementAt('Entity', i),
            value: query.getElementAt(todoItemComponentName, i).Content
        });
    }

    const addItem = () => {
        const newValue = new TodoItemComponent();
        newValue.Content = value;

        executeBuffer((buffer) => {
            const entity = buffer.createEntity();
            buffer.setComponent(todoItemComponentName, entity, newValue);
        })

        setValue('');
    }

    const removeItem = (entity) => {
        executeBuffer((buffer) => {
            buffer.destroyEntity(entity);
        });
    }

    return (
        <visualElement style={{ height: '100%' }}>
            <scrollView style={{ height: '100%' }}>
                {items.map((e, i) => <TodoItem key={i} value={e.value} onRemove={() => removeItem(e.entity)} />)}
            </scrollView>
            <textField value={value} onChange={newValue => setValue(newValue)} />
            <Button style={{ height: 70, minHeight: 70, alignSelf: 'FlexEnd' }} onMouseUpEvent={addItem}>
                Add TODO item
            </Button>
        </visualElement>
    );
}