import { Tabs } from '@chakra-ui/react'
import type { AssignmentResult } from '../types/assignment'

interface Props {
	hungarianResult: AssignmentResult | null
	auctionFixedResult: AssignmentResult | null
	auctionScaledResult: AssignmentResult | null
	selectedTab: string | null
	epsilonPrecision: number | null
	setSelectedTab: (tab: string) => void
}

export default function AssignmentResultCard({
	hungarianResult,
	auctionFixedResult,
	auctionScaledResult,
	selectedTab,
	epsilonPrecision,
	setSelectedTab,
}: Props) {
	const hasHungarian = !!hungarianResult
	const hasAuctionFixed = !!auctionFixedResult
	const hasAuctionScaled = !!auctionScaledResult

	if (!hasHungarian && !hasAuctionFixed && !hasAuctionScaled) return null
	return (
		<Tabs.Root
			lazyMount
			unmountOnExit
			value={selectedTab}
			variant='outline'
			onValueChange={e => setSelectedTab(e.value)}
		>
			<Tabs.List bg='white' mb='2' gap='2' fontSize='sm'>
				{hasHungarian && (
					<Tabs.Trigger
						value='hungarian'
						px='1'
						py='2'
						fontWeight='bold'
						borderBottom='2px solid'
						borderColor='transparent'
						_selected={{
							borderColor: 'blue.600',
							transition: 'border-color 0.25s ease',
						}}
					>
						Hungarian
					</Tabs.Trigger>
				)}

				{hasAuctionFixed && (
					<Tabs.Trigger
						value='auction-fixed'
						px='1'
						py='2'
						fontWeight='bold'
						borderBottom='2px solid'
						borderColor='transparent'
						_selected={{
							borderColor: 'blue.600',
							transition: 'border-color 0.25s ease',
						}}
					>
						Auction Fx.
					</Tabs.Trigger>
				)}

				{hasAuctionScaled && (
					<Tabs.Trigger
						value='auction-scaled'
						px='1'
						py='2'
						fontWeight='bold'
						borderBottom='2px solid'
						borderColor='transparent'
						_selected={{
							borderColor: 'blue.600',
							transition: 'border-color 0.25s ease',
						}}
					>
						Auction Sc.
					</Tabs.Trigger>
				)}
			</Tabs.List>

			{hasHungarian && (
				<Tabs.Content value='hungarian'>
					<ResultPanel result={hungarianResult!} />
				</Tabs.Content>
			)}

			{hasAuctionFixed && (
				<Tabs.Content value='auction-fixed'>
					<ResultPanel result={auctionFixedResult!} />
				</Tabs.Content>
			)}

			{hasAuctionScaled && (
				<Tabs.Content position='relative' value='auction-scaled'>
					{epsilonPrecision != null && (
						<div className='absolute top-4 right-3 text-xs text-gray-500 select-none'>
							ε precision:{' '}
							<span className='select-text'>{epsilonPrecision}</span>
						</div>
					)}
					<ResultPanel result={auctionScaledResult!} />
				</Tabs.Content>
			)}
		</Tabs.Root>
	)
}

function ResultPanel({ result }: { result: AssignmentResult }) {
	const formatTime = (ms: number) => {
		const totalSeconds = Math.floor(ms / 1000)
		const minutes = Math.floor(totalSeconds / 60)
		const seconds = totalSeconds % 60
		const hundredths = Math.floor((ms % 1000) / 10)

		return `${minutes}:${seconds.toString().padStart(2, '0')}.${hundredths
			.toString()
			.padStart(2, '0')}`
	}

	const formatMemory = (bytes: number) => {
		if (bytes >= 1024 * 1024)
			return `${(bytes / 1024 / 1024).toLocaleString('en-US')} MB`
		if (bytes >= 1024) return `${(bytes / 1024).toLocaleString('en-US')} KB`
		return `${bytes.toLocaleString('en-US')} B`
	}

	const formatDistance = (meters: number, threshold = 1000) => {
		if (meters >= threshold) return `${(meters / 1000).toFixed(4)} km`
		return `${meters.toFixed(0)} m`
	}

	return (
		<div className='bg-gray-50 border border-gray-200 rounded-2xl shadow-sm p-5 w-full max-w-md'>
			<h3 className='text-lg font-semibold text-gray-800 mt-1 mb-4 text-center'>
				Assignment Results
			</h3>

			<div className='space-y-3 text-base mb-1 text-gray-700'>
				<div className='flex justify-between'>
					<span>Execution time:</span>
					<span className='font-medium'>
						{formatTime(result.executionTimeMs)}
					</span>
				</div>

				<div className='flex justify-between'>
					<span>Memory used:</span>
					<span className='font-medium'>
						{formatMemory(result.memoryUsedBytes)}
					</span>
				</div>

				<div className='flex justify-between'>
					<span>Total distance:</span>
					<span className='font-medium'>
						{formatDistance(result.totalDistanceMeters)}
					</span>
				</div>
			</div>
		</div>
	)
}
