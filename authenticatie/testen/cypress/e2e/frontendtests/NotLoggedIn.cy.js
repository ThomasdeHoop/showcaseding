describe('Navigation to Chat Page', () => {
    beforeEach(() => {
        // Visit the homepage
        cy.visit('https://localhost:7074/');
    });

    it('Selects "Chat" and redirects to login page', () => {
        // Click on the "Chat" link or button that leads to the login page
        cy.contains('Chat').click();

        // Check if redirected to the login page
        cy.url().should('include', '/Identity/Account/Login');
    });
});
