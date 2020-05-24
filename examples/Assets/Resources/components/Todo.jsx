import React, { useState } from 'react';
import Button from './button';
import { useQuery, createEntity, removeEntity } from 'unity-renderer'; 

function TodoItem({ value, onRemove }) {

    return (
        <Button innerStyle={{ flexDirection: 'row', justifyContent: 'space-between' }} style={{ height: 70 }}>
            <visualElement style={{ marginLeft: 10 }}>{value}</visualElement>
            <Button onMouseUpEvent={onRemove} style={{ width: 70, height: 50, marginRight: 5 }}>Delete</Button>
        </Button>
    )
}

export default function Todo() {
    const [isInitializing, todoItems] = useQuery('TodoItems');
    const [value, setValue] = useState('');

    const addItem = () => {
        const utf8 = unescape(encodeURIComponent(value));

        const arr = [];
        for (let i = 0; i < utf8.length; i++) {
            arr.push(utf8.charCodeAt(i));
        }

        createEntity([['TodoItemComponent', {
            data: arr
        }]])

        setValue('');
    }

    const mapArrayToString = (arr) => {
        if (!arr) return '';

        const utf8 = String.fromCharCode(...arr);
        return decodeURIComponent(escape(utf8));
    }

    const removeItem = (index) => {
        removeEntity(index, [ 'TodoItemComponent' ]);
    }

    const items = isInitializing
        ? []
        : todoItems.map(e => mapArrayToString(e.data));

    return (
        <visualElement style={{ height: '100%' }}>
            <scrollview style={{ height: '100%' }}>
                {items.map((e, i) => <TodoItem key={i} value={e} onRemove={() => removeItem(i)} />)}
            </scrollview>
            <textfield text={value} onChange={e => setValue(e.value)} />
            <Button style={{ height: 70, minHeight: 70, alignSelf: 'flex-end' }} onMouseUpEvent={addItem}>
                Add TODO item
            </Button>
        </visualElement>
    );
}