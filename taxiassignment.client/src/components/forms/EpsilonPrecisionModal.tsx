import {
	Button,
	CloseButton,
	Dialog,
	Portal,
	Input,
	Slider,
	useSlider,
	Stack,
} from '@chakra-ui/react'
import { useState } from 'react'

interface EpsilonDialogProps {
	isOpen: boolean
	onClose: () => void
	onApply: (precision: number) => void
}

export default function EpsilonPrecisionDialog({
	isOpen,
	onClose,
	onApply,
}: EpsilonDialogProps) {
	const [epsilon, setEpsilon] = useState(0.5)

	const slider = useSlider({
		min: 0,
		max: 1,
		step: 0.01,
		value: [epsilon],
		thumbAlignment: 'center',
		onValueChange: details => {
			setEpsilon(details.value[0])
		},
	})

	return (
		<Dialog.Root
			open={isOpen}
			onOpenChange={onClose}
			motionPreset='slide-in-left'
			size='md'
		>
			<Portal>
				<Dialog.Backdrop className='bg-black/50 backdrop-blur-sm' />
				<Dialog.Positioner>
					<Dialog.Content className='rounded-xl bg-white text-black'>
						<Dialog.Header justifyContent='center' marginBottom='2'>
							<Dialog.Title fontSize='2xl' fontWeight='bold'>
								Select ε-scaling precision
							</Dialog.Title>
						</Dialog.Header>

						<Dialog.Body marginBottom='0.5'>
							<form
								onSubmit={e => {
									e.preventDefault()
									onApply(epsilon)
									onClose()
								}}
							>
								<Stack gap={4} align='center'>
									<Input
										type='number'
										min={0}
										max={1}
										step={0.01}
										value={epsilon}
										paddingEnd='1'
										textAlign='center'
										fontSize='lg'
										fontWeight='medium'
										onChange={e => setEpsilon(Number(e.target.value))}
										className='w-32'
									/>

									<Slider.RootProvider
										value={slider}
										colorPalette='blue'
										size='lg'
										className='w-full'
									>
										<Slider.Control>
											<Slider.Track bgColor='gray.300'>
												<Slider.Range />
											</Slider.Track>
											<Slider.Thumb
												index={0}
												className='bg-blue-600'
												boxSize='5'
											/>
										</Slider.Control>
									</Slider.RootProvider>

									<Dialog.Footer justifyContent='center'>
										<Button
											type='submit'
											bgColor='green.solid'
											color='white'
											_hover={{ bg: 'green.emphasized' }}
											_active={{ bg: 'green.800' }}
											_disabled={{ bg: 'green.700' }}
											transition='background-color 0.35s ease'
											fontWeight='bold'
											fontSize='lg'
											padding='3'
										>
											Run Algorithm
										</Button>
									</Dialog.Footer>
								</Stack>
							</form>
						</Dialog.Body>

						<Dialog.CloseTrigger asChild>
							<CloseButton size='xl' color='red.600' />
						</Dialog.CloseTrigger>
					</Dialog.Content>
				</Dialog.Positioner>
			</Portal>
		</Dialog.Root>
	)
}
