import { jest } from '@jest/globals';
import { handler } from './index.mjs';

// Mock the utils.mjs functions
jest.mock('../../shared/utils.mjs', () => ({
  getSecret: jest.fn(() => Promise.resolve({ username: 'user', password: 'pass' })),
  createDbClient: jest.fn(() => ({
    connect: jest.fn(),
    query: jest.fn((query, values) => {
      // Mock database responses based on the updated user data
      if (query.includes('SELECT items_owned')) {
        if (values[0] === 'emp_001') return { rows: [{ items_owned: [200, 100], items_equipped: [] }] };
        if (values[0] === 'emp_003') return { rows: [{ items_owned: [], items_equipped: [] }] };
        if (values[0] === '681278') return { rows: [{ items_owned: [], items_equipped: [] }] };
        return { rows: [] };
      }
      if (query.includes('SELECT * FROM users WHERE employee_id')) {
        if (values[0] === 'emp_001') return { rows: [{ employee_id: 'emp_001', current_coins: 900, total_coins: 1000 }] };
        if (values[0] === 'emp_002') return { rows: [{ employee_id: 'emp_002', current_coins: 50, total_coins: 150 }] };
        if (values[0] === 'emp_003') return { rows: [{ employee_id: 'emp_003', current_coins: 0, total_coins: 0 }] };
        if (values[0] === '681278') return { rows: [{ employee_id: '681278', current_coins: 1, total_coins: 1 }] };
        return { rows: [] };
      }
      return { rows: [] };
    }),
    end: jest.fn(),
  })),
}));

// Mock event data
const mockEventValid = {
  body: JSON.stringify({
    action: 'equip',
    items: [200, 100], // Items that the user owns
    employee_id: 'emp_001', // Valid employee_id with owned items
  }),
};

const mockEventInvalid = {
  body: JSON.stringify({
    action: 'equip',
    items: [300], // Item not owned by the user
    employee_id: 'emp_003', // Valid employee_id but with no owned items
  }),
};

const mockEventNonExistent = {
  body: JSON.stringify({
    action: 'equip',
    items: [100], // Any item
    employee_id: 'non_existent_emp', // Non-existent employee_id
  }),
};

describe('Equip Handler Tests', () => {
  it('should successfully equip items for a user with valid items', async () => {
    const response = await handler(mockEventValid);
    expect(response.statusCode).toBe(200);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.message).toContain('Successfully equipped');
  });

  it('should throw an error for a non-existent employee_id', async () => {
    const response = await handler(mockEventNonExistent);
    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toBe('User not found with the specified employee_id.');
  });

  it('should throw an error if the user does not own the specified items', async () => {
    const response = await handler(mockEventInvalid);
    expect(response.statusCode).toBe(400);
    const responseBody = JSON.parse(response.body);
    expect(responseBody.error).toContain('User does not own these items');
  });
});
