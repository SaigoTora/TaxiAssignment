import {
	Button,
	createListCollection,
	Input,
	Portal,
	Select,
} from '@chakra-ui/react'
import { useState } from 'react'
import type { City, GenerateDataFormProps } from '../types/forms'

export default function GenerateDataForm({
	onGenerate,
}: GenerateDataFormProps) {
	const [inputData, setInputData] = useState<{
		city: City
		taxiDriversCount?: number
		clientCount?: number
	}>({
		city: 'Kyiv',
		taxiDriversCount: 10,
		clientCount: 10,
	})

	const cities = createListCollection({
		items: [
			{ label: 'Kyiv', value: 'Kyiv' },
			{ label: 'Kharkiv', value: 'Kharkiv' },
			{ label: 'Lviv', value: 'Lviv' },
		],
	})

	const onSubmit = (e: React.FormEvent<HTMLFormElement>) => {
		e.preventDefault()
		onGenerate(inputData)
	}

	return (
		<form onSubmit={onSubmit} className='flex w-full flex-col'>
			<h3 className='text-xl font-bold mb-6 text-center'>Data generation</h3>

			<div className='flex flex-col mb-4'>
				<label className='font-medium mb-1 cursor-text'>City:</label>
				<Select.Root
					collection={cities}
					value={[inputData.city]}
					onValueChange={({ value }) =>
						setInputData({ ...inputData, city: value[0] as City })
					}
				>
					<Select.HiddenSelect />
					<Select.Control className='border-2 border-gray-300 rounded-lg px-2'>
						<Select.Trigger>
							<Select.ValueText placeholder='Select city' />
						</Select.Trigger>
						<Select.IndicatorGroup>
							<Select.Indicator />
						</Select.IndicatorGroup>
					</Select.Control>

					<Portal>
						<Select.Positioner borderColor='blackAlpha.700'>
							<Select.Content className='bg-white text-black rounded-lg shadow-md'>
								{cities.items.map(city => (
									<Select.Item
										key={city.value}
										item={city}
										className='px-3 py-2 cursor-pointer rounded-md'
									>
										{city.label}
										<Select.ItemIndicator />
									</Select.Item>
								))}
							</Select.Content>
						</Select.Positioner>
					</Portal>
				</Select.Root>
			</div>

			<label className='font-medium mb-1 cursor-text'>
				Number of taxi drivers:
			</label>
			<Input
				type='number'
				placeholder='Number of taxi drivers'
				paddingEnd='1'
				paddingStart='1.5'
				marginBottom='4'
				value={inputData.taxiDriversCount ?? ''}
				min={1}
				max={10000}
				onChange={e =>
					setInputData({
						...inputData,
						taxiDriversCount: Number(e.target.value),
					})
				}
			/>

			<label className='font-medium mb-1 cursor-text'>Number of clients:</label>
			<Input
				type='number'
				placeholder='Number of clients'
				paddingEnd='1'
				paddingStart='1.5'
				marginBottom='6'
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
