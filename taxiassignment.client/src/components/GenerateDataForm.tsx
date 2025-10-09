import { Button, Input } from '@chakra-ui/react'
import { useState } from 'react'
import type { GenerateDataFormProps } from '../types/forms'

export default function GenerateDataForm({
    onGenerate,
}: GenerateDataFormProps) {
    const [inputData, setInputData] = useState<{
        taxiDriversCount?: number
        clientCount?: number
    }>({
        taxiDriversCount: 10,
        clientCount: 10,
    })

    const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        onGenerate(inputData)
    }

    return (
        <form onSubmit={onSubmit} className='flex w-full flex-col gap-2'>
            <h3 className='text-xl font-bold'>Data generation</h3>
            <label>Number of taxi drivers:</label>
            <Input
                type='number'
                placeholder='Number of taxi drivers'
                paddingEnd='1'
                paddingStart='1.5'
                marginBottom='2'
                value={inputData.taxiDriversCount ?? ''}
                min={1}
                max={10000}
                onChange={e =>
                    setInputData({ ...inputData, taxiDriversCount: Number(e.target.value) })
                }
            />

            <label>Number of clients:</label>
            <Input
                type='number'
                placeholder='Number of clients'
                paddingEnd='1'
                paddingStart='1.5'
                value={inputData.clientCount ?? ''}
                min={1}
                max={10000}
                onChange={e =>
                    setInputData({ ...inputData, clientCount: Number(e.target.value) })
                }
            />
            <Button
                type='submit'
                bgColor='yellow.500'
                fontWeight='bold'
                fontSize='md'
            >
                Generate
            </Button>
        </form>
    )
}
