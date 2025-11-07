import { Button } from '@chakra-ui/react'
import { useState } from 'react'

interface AssignmentButtonsProps {
	onHungarianAssign: () => Promise<void> | void
	onAuctionFixedAssign: () => Promise<void> | void
	onAuctionScaledAssign: () => Promise<void> | void
	onLoadingChange?: (isLoading: boolean) => void
}

export default function AssignmentButtons({
	onHungarianAssign,
	onAuctionFixedAssign,
	onAuctionScaledAssign,
	onLoadingChange,
}: AssignmentButtonsProps) {
	const [isLoading, setIsLoading] = useState(false)

	const handleClick = async (action: () => Promise<void> | void) => {
		setIsLoading(true)
		onLoadingChange?.(true)

		try {
			await action()
		} finally {
			setIsLoading(false)
			onLoadingChange?.(false)
		}
	}

	return (
		<div className='flex w-full flex-col'>
			<h3 className='text-xl font-bold mb-3 text-center'>Assignment</h3>

			<Button
				onClick={() => handleClick(onHungarianAssign)}
				bgColor='green.solid'
				color='white'
				_hover={{ bg: 'green.emphasized' }}
				_active={{ bg: 'green.800' }}
				_disabled={{ bg: 'green.700' }}
				transition='background-color 0.35s ease'
				fontWeight='bold'
				fontSize='md'
				marginBottom='2'
				disabled={isLoading}
			>
				{isLoading ? 'Processing...' : '🧮 Hungarian Algorithm'}
			</Button>

			<Button
				onClick={() => handleClick(onAuctionFixedAssign)}
				bgColor='green.solid'
				color='white'
				_hover={{ bg: 'green.emphasized' }}
				_active={{ bg: 'green.800' }}
				_disabled={{ bg: 'green.700' }}
				transition='background-color 0.35s ease'
				fontWeight='bold'
				fontSize='md'
				marginBottom='2'
				disabled={isLoading}
			>
				{isLoading ? 'Processing...' : '⚖️ Auction (Fixed ε)'}
			</Button>

			<Button
				onClick={() => handleClick(onAuctionScaledAssign)}
				bgColor='green.solid'
				color='white'
				_hover={{ bg: 'green.emphasized' }}
				_active={{ bg: 'green.800' }}
				_disabled={{ bg: 'green.700' }}
				transition='background-color 0.35s ease'
				fontWeight='bold'
				fontSize='md'
				disabled={isLoading}
			>
				{isLoading ? 'Processing...' : '📈 Auction (ε-Scaling)'}
			</Button>
		</div>
	)
}
