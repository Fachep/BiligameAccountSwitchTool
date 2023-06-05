using BiligameAccountSwitchTool.Helpers;
using BiligameAccountSwitchTool.Models;
using BiligameAccountSwitchTool.Services;
using BiligameAccountSwitchTool.Views;
using System.ComponentModel;

namespace BiligameAccountSwitchTool
{
    public partial class MainWindow : Form
    {
        private readonly GameService _gameservice = AppConfig.GetService<GameService>();
        private readonly AccountService _accountService = AppConfig.GetService<AccountService>();
        private readonly ObservableWarpper<BindingList<GameAccount>> _accounts = new(new());
        private readonly ObservableWarpper<BindingList<Game>> _games;
        private readonly BindingSource _bsGames;
        private readonly BindingSource _bsAccounts;

        private ManageAccount? manageAccountWindow;
        private ManageGame? manageGameWindow;
        private HelpWindow? helpWindow;

        public MainWindow()
        {
            _games = new ObservableWarpper<BindingList<Game>>(new BindingList<Game>(_gameservice.AllGamesList));
            InitializeComponent();
            _bsGames = new BindingSource(new BindingSource(_games, null), "Value");
            _bsAccounts = new BindingSource(new BindingSource(_accounts, null), "Value");
            comboBoxGame.DisplayMember = "Name";
            comboBoxGame.DataSource = _bsGames;
            comboBoxAccount.DisplayMember = "Name";
            comboBoxAccount.DataSource = _bsAccounts;
            if (IEVersionHelper.Version < new Version(9, 0, 0, 0))
            {
                MessageBox.Show("Internet Explorer 版本过低，仅支持 IE 9 以上版本", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void buttonChangeAccount_Click(object sender, EventArgs e)
        {
            if (comboBoxAccount.SelectedValue == null) return;
            if (comboBoxGame.SelectedValue == null) return;
            _accountService.ApplyAccount((GameAccount)comboBoxAccount.SelectedValue);
        }

        private void buttonManageAccount_Click(object sender, EventArgs e)
        {
            if (comboBoxGame.SelectedValue == null) return;
            manageAccountWindow ??= new ManageAccount(comboBoxGame, _accounts);
            manageAccountWindow.ShowDialog();
            _accounts.Value.Remove(GameAccount.newAccount);
        }

        private void comboBoxGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGame.SelectedValue == null) return;
            _accounts.Value = new BindingList<GameAccount>(_accountService.GetAccountsFromDatabase(((Game)comboBoxGame.SelectedValue).Id));
        }

        private void buttonManageGame_Click(object sender, EventArgs e)
        {
            manageGameWindow ??= new ManageGame();
            manageGameWindow.ShowDialog();
            _games.Value = new BindingList<Game>(_gameservice.AllGamesList);
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            if (helpWindow?.IsDisposed != false) helpWindow = new HelpWindow();
            helpWindow.Show();
            helpWindow.Focus();
        }
    }
}