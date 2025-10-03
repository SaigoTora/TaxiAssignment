import { Button, Input } from '@chakra-ui/react'
import { useState } from 'react'
import type { GenerateDataFormProps } from '../types/forms'

export default function GenerateDataForm({
    onGenerate,
}: GenerateDataFormProps) {
    const [inputData, setInputData] = useState<{
        taxiCount?: number
        clientCount?: number
    }>({
        taxiCount: 10,
        clientCount: 10
    })

    const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        onGenerate(inputData)
    }

    return (
        <form onSubmit={onSubmit} className='flex w-full flex-col gap-3'>
            <h3 className='text-xl font-bold'>Data generation</h3>
            <Input
                type='number'
                placeholder='Number of taxis'
                value={inputData.taxiCount ?? ''}
                min={0}
                max={10000}
                onChange={e =>
                    setInputData({ ...inputData, taxiCount: Number(e.target.value) })
                }
            />
            <Input
                type='number'
                placeholder='Number of clients'
                value={inputData.clientCount ?? ''}
                min={0}
                max={10000}
                onChange={e =>
                    setInputData({ ...inputData, clientCount: Number(e.target.value) })
                }
            />
            <Button type='submit' colorScheme='teal'>
                Generate
            </Button>
        </form>
    )
}
